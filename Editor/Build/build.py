import base64
import hashlib
import hmac
import json
import os
import re
import subprocess
import sys
import time
import urllib.parse
import requests
import pathlib
import tempfile

project_path = sys.argv[1]
is_debug = sys.argv[2]
up_version = sys.argv[3]
platform = sys.argv[4]
use_hotfix = sys.argv[5]
use_aab = sys.argv[6]
# stable = sys.argv[7]
stable = "False"
build_path_relative_project = "../../share/build"
build_path = project_path + "/" + build_path_relative_project
svn_commit_path = build_path
svn_account = "--username jenkins --password jenkins"

editor_path = "D:/unity/Unity/2020.3.18f1c1/Editor/Unity.exe"


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

        # set environment
        self.args = []
        self.args.append('--BUILDPATH:"{}"'.format(build_path_relative_project))
        self.args.append("--DEBUG:" + is_debug)
        self.args.append("--UPVERSION:" + up_version)
        self.args.append("--PLATFORM:" + platform)
        self.args.append("--USEHOTFIX:" + use_hotfix)
        self.args.append("--AAB:" + use_aab)
        self.args.append("--STABLE:" + stable)

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
        buildFunction = "Framework.Editor.Build.JenkinsBuild"
        UnityBuildCmd = "{} -batchmode -quit -nographics -executeMethod {} -logFile {} -projectPath {} {}".format(
            editor_path, buildFunction, self.LogFilePath, project_path, " ".join(self.args))
        LogConsole(UnityBuildCmd)
        ExecuteShellWithFile(UnityBuildCmd)


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
    result = subprocess.check_output(command, shell=True)
    try:
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


def notify(msg, success):
    url = 'https://oapi.dingtalk.com/robot/send?access_token=55bce92dc73ad58bc7ba2ae8590205d6c7a5fae3b936cdf45769fc7adb929e50'
    timestamp = str(round(time.time() * 1000))
    secret = 'SECa482adba43f33bae1d7ff98181f7a50c208c3786012461a4887a97302d0ae418'
    secret_enc = secret.encode('utf-8')
    string_to_sign = '{}\n{}'.format(timestamp, secret)
    string_to_sign_enc = string_to_sign.encode('utf-8')
    hmac_code = hmac.new(secret_enc, string_to_sign_enc, digestmod=hashlib.sha256).digest()
    sign = urllib.parse.quote_plus(base64.b64encode(hmac_code))
    url = url + "&timestamp={0}&sign={1}".format(timestamp, sign)
    headers = {'Content-Type': 'application/json;charset=utf-8'}
    title = ""
    if success == 0:
        title = "打包成功"
    else:
        title = "打包失败"
    data = {
        # 发送消息类型为文本
        "msgtype": "text",
        "at": {
            # 决定是否@所有人,True为发送的消息自动@所有人
            "isAtAll": success != 0,
        },
        "text": {
            "title": title,
            # 消息正文
            "content": msg
        }
    }
    r = requests.post(url, data=json.dumps(data), headers=headers)
    return r.text


result = update_svn()
result = ""
obj = BaseBuilder()
obj.Build()
success = obj.CheckBuildState()
commit_svn()
LogConsole(result)
notify_msg = "is_debug={} up_version={} platform={} use_hotfix={}".format(is_debug, up_version, platform, use_hotfix)
if success == 1:
    notify("打包失败\n {} \n{} \n {} \n 检查Log看看错误\n{}".format(notify_msg, result, obj.Error, obj.LogFilePath), success)
    exit(1)
if platform == "Android":
    result = "手机下载地址 http://192.168.10.112:1980 \n" + result
notify("打包成功\n{}\n{}".format(notify_msg, result), success)
commit_git()
exit(0)
