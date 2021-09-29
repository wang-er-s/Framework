import os
import subprocess
import sys
import re
import io
import codecs
    
project_path = sys.argv[1]
is_debug = sys.argv[2]
up_version = sys.argv[3]
platform = sys.argv[4]
use_hotfix = sys.argv[5]
use_aab = sys.argv[6]
svn_path = sys.argv[7]
build_path = project_path + "/Build"
svn_commit_path = build_path

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

# 基类
class BaseBuilder:
    def __init__(self):
        # get base environment
        self.UnityAppPath = "C:/Program\" \"Files/Unity/Hub/Editor/2019.4.24f1c1/Editor/Unity.exe"
        self.LogFilePath = build_path+"/buildLog.log"
        self.Error = ""

        #set environment
        self.args = []
        self.args.append("--BUILDPATH:"+build_path)
        self.args.append("--DEBUG:"+is_debug)
        self.args.append("--UPVERSION:"+up_version)
        self.args.append("--PLATFORM:"+platform)
        self.args.append("--USEHOTFIX:"+use_hotfix)
        self.args.append("--AAB:"+use_aab)

    def CheckBuildState(self):
        f = open(self.LogFilePath,"r",encoding="utf-8")
        lines = f.readlines()
        last_line = lines[-1]
        if(last_line.find("1") >= 0):
            for line in lines:
                if(line.find("error")) >= 0:
                    self.Error += line
        
    @SafeHandleFunction("Begin to build UnityProject", errorlog="Build UnityProject Error", errorcode=-1)
    def Build(self):
        buildFunction = "Framework.Editor.Build.JenkinsBuild"
        UnityBuildCmd = "{} -batchmode -quit -nographics -executeMethod {} -logFile {} -projectPath {} {}".format(
            self.UnityAppPath, buildFunction, self.LogFilePath, project_path, " ".join(self.args))
        ExecuteBuildShell(UnityBuildCmd)

    

def LogConsole(content, isError=False):
    print('\n<--- ' + str(content) + ' --->')
    return
    if not isError:
        sys.stdout.write('\n<--- ' + str(content) + ' --->')
        sys.stdout.flush()
        print('\n<--- ' + str(content) + ' --->')
    else:
        sys.stderr.write('\n<--- ' + str(content) + ' --->')
        sys.stderr.flush()

def ExecuteBuildShell(command):
    shell_name = "tmp.sh"
    f=open(shell_name,"w")
    command="`"+command+"`"
    LogConsole(command)
    f.write(command)
    f.close()
    result = 1
    result = subprocess.call(shell_name, executable=None, shell=True)
    LogConsole("result: " + str(result))
    os.remove(shell_name)
    return result

def ExecuteShell(command):
    LogConsole(command)
    result = subprocess.check_output(command,shell=True)
    try:
        result = result.decode('gbk').strip('\r\n')  #处理GBK编码的输出，去掉结尾换行
    except:
        result = result.decode('utf-8').strip('\r\n')  #如果GBK解码失败再尝试UTF-8解码
    return result

def update_svn():
    result= ExecuteShell("svn info {} --show-item revision".format(svn_path))
    old_version = int(result)
    ExecuteShell("svn update {}".format(svn_path))
    result= ExecuteShell("svn info {} --show-item revision".format(svn_path))
    new_version = int(result)
    if(old_version != new_version):
        update_info = ExecuteShell("svn log {} -r {}:{}".format(svn_path, old_version, new_version))
        return update_info
    return ""

def commit_svn():
    file_status = ExecuteShell("svn status {}".format(svn_commit_path))
    status = file_status.split("\n")
    need_del_file_path = []
    for s in status:
        if(len(s) > 1):
            match = re.match(r"(.*?)(\s+?)(\w+[\d\D]*)",s)
            if(match.group(1) == "!"):
                need_del_file_path.append(match.group(3))
    for del_file in need_del_file_path:
        ExecuteShell("svn delete {}".format(del_file.strip()))
    ExecuteShell("svn add {} --force".format(svn_commit_path))
    ExecuteShell('svn commit {} -m "commit build ...."'.format(svn_commit_path))

result = update_svn()
obj = BaseBuilder()
obj.Build()
obj.CheckBuildState()
commit_svn()
LogConsole(result)
if(obj.Error != ""):
    raise Exception(obj.Error)
    exit(1)
exit(0)