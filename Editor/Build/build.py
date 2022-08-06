import os
import re
import subprocess
import sys
import time
import pathlib
import tempfile

class build_types():
    BuildDll = "1"
    BuildAB = "2"
    BuildPlayer = "3"

build_type = sys.argv[1]
project_path = sys.argv[2]
is_debug = "false"
use_hotfix = "false"
platform = "Android"
incremental = "false"

if build_type == build_types.BuildDll:
    platform = "Android"
elif build_type == build_types.BuildAB:
    platform = sys.argv[3]
    incremental = sys.argv[4]
elif build_type == build_types.BuildPlayer:
    is_debug = sys.argv[3]
    platform = sys.argv[4]
    use_hotfix = sys.argv[5]

build_path_relative_project = "../../share/build"
build_path = project_path + "/" + build_path_relative_project
svn_commit_path = build_path
svn_account = "--username jenkins --password jenkins"
editor_path = "D:/unity/Unity/2021.3.6f1c1/Editor/Unity.exe"

def SafeHandleFunction(log='', errorlog='', errorcode=-1):
    def decorator(func):
        def wrapper(*args, **kwargs):
            if len(log) != 0:
                LogConsole(log)
            try:
                result = func(*args, **kwargs)
                return result
            except Exception as ex:
                if len(errorlog) != 0:
                    LogConsole(errorlog, 1)
                LogConsole(str(ex), 1)
                exit(errorcode)
            return func

        return wrapper

    return decorator


def LogConsole(content):
    print('\n<--- ' + str(content) + ' --->')


# 基类
class BaseBuilder:
    def __init__(self):
        # get base environment
        self.LogFilePath = build_path + "/buildLog.log"
        self.Error = ""
        self.buildFunction = ""
        # set environment
        self.args = []
        self.args.append('--BUILDPATH:"{}"'.format(build_path_relative_project))
        self.args.append("--DEBUG:" + is_debug)
        self.args.append("--PLATFORM:" + platform)
        self.args.append("--USEHOTFIX:" + use_hotfix)
        self.args.append("--IncrementalBuild:"+incremental)

    def CheckBuildState(self):
        f = open(self.LogFilePath, "r", encoding="utf-8")
        lines = f.readlines()
        last_line = lines[-1]
        if last_line.find("1") >= 0:
            start_save_error = False
            for line in lines:
                if (start_save_error):
                    self.Error += line
                    continue
                if (line.find("BuildException")) >= 0:
                    self.Error += line
                    start_save_error = True

            if (start_save_error == False):
                for line in lines:
                    if (line.find("): error ")) >= 0:
                        self.Error += line

            return 1
        return 0

    @SafeHandleFunction("Begin to build UnityProject", errorlog="Build UnityProject Error", errorcode=-1)
    def Build(self):
        if build_type == build_types.BuildPlayer:
            self.BuildPlayer()
        elif build_type == build_types.BuildDll:
            self.BuildDll()
        elif build_type == build_types.BuildAB:
            self.BuildAb()
        UnityBuildCmd = "{} -batchmode -quit -nographics -buildTarget {} -executeMethod {} -logFile {} -projectPath {} {}".format(
            editor_path,platform, self.buildFunction, self.LogFilePath, project_path, " ".join(self.args))
        LogConsole(UnityBuildCmd)
        ExecuteShellWithFile(UnityBuildCmd)

    def BuildAb(self):
        self.buildFunction = "Framework.Editor.Build.JenkinsBuildCodeAndAb"

    def BuildPlayer(self):
        self.buildFunction = "Framework.Editor.Build.JenkinsBuildAll"

    def BuildDll(self):
        self.buildFunction = "Framework.Editor.Build.JenkinsBuildCode"


def ExecuteShellWithFile(command):
    shell_name = os.path.join(tempfile.gettempdir(), str(time.time()) + ".sh")
    f = open(shell_name, "w")
    command = "`" + command + "`"
    LogConsole(command)
    f.write(command)
    f.close()
    result = subprocess.call(shell_name, executable=None, shell=True)
    LogConsole("result: " + str(result))
    os.remove(shell_name)
    return result


def ExecuteShell(command):
    LogConsole(command)
    result = ""
    try:
        result = subprocess.check_output(command, shell=True)
        result = result.decode('gbk').strip('\r\n')  # 处理GBK编码的输出，去掉结尾换行
    except:
        try:
            result = result.decode('utf-8').strip('\r\n')  # 如果GBK解码失败再尝试UTF-8解码
        except:
            result = ""
    return result


def get_svn_root():
    def find_file():
        for file in os.listdir("."):
            if file == "root_dir":
                return True
        return False

    cur_path = pathlib.Path(__file__).parent.resolve()
    while True:
        cur_path = os.path.dirname(cur_path)
        os.chdir(cur_path)
        if find_file():
            return cur_path


def update_svn():
    svn_path = get_svn_root()
    result = ExecuteShell("svn info {} --show-item revision".format(svn_path))
    old_version = int(result)
    ExecuteShell("svn update {} {}".format(svn_path, svn_account))
    result = ExecuteShell("svn info {} --show-item revision".format(svn_path))
    new_version = int(result)
    if (old_version != new_version):
        update_info = ExecuteShell("svn log {} -r {}:{}".format(svn_path, old_version, new_version))
        return update_info
    return ""


def commit_svn():
    file_status = ExecuteShell("svn status {}".format(svn_commit_path))
    status = file_status.split("\n")
    need_del_file_path = []
    for s in status:
        if len(s) > 1:
            match = re.match(r"(.*?)(\s+?)(\w+[\d\D]*)", s)
            if match.group(1) == "!":
                need_del_file_path.append(match.group(3))
    for del_file in need_del_file_path:
        ExecuteShell("svn delete {} {}".format(del_file.strip(), svn_account))
    ExecuteShell("svn add {} --force {}".format(svn_commit_path, svn_account))
    ExecuteShell('svn commit {} -m "commit build ...." {}'.format(svn_commit_path, svn_account))


def commit_git():
    ExecuteShellWithFile("""
cd /d {}
git add .
git commit -m "Add changed file"
git push origin master
    """.format(project_path))

update_svn()
obj = BaseBuilder()
obj.Build()
success = obj.CheckBuildState()
commit_svn()
f = open(obj.LogFilePath, 'r',  encoding='utf8')
# LogConsole(f.read())
if success == 1:
    exit(1)
# commit_git()
exit(0)

