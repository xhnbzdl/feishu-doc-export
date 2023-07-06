# feishu-doc-export

## 动机

最近也是公司办公软件从飞书切换回了企业微信，自然就产生了一些文档要迁移的问题，由于文档量过多（大概有700多个），无论是从飞书手动下载为Word或PDF格式的文档，还是将内容复制到本地新建Markdown文件都是一件极为繁琐的事情。于是便找到了两个GitHub上已有的飞书文档导出工具`Feishu2MD`和`feishu-backup`，但是他们都有一些问题不太满足我的需求。

### 现有方案的不满足

**feishu-backup：**

官方地址：[dicarne/feishu-backup: 用于备份飞书文档，可以将飞书文档转成markdown下载。 (github.com)](https://github.com/dicarne/feishu-backup)

1. 因为它是网页版，下载速度太慢。有一次使用线上版选择了其中一个飞书文档节点下的所有文档（大概200-300个），下载了1个多小时还没有好，可能是卡死了。

2. 因为它的下载方式是把选择的全部文档打包成压缩包后才会在浏览器返回给你，如果这个等待的过程中途断网或者电脑卡顿要重启，那你就白等那么长时间了。

3. 因为它不支持下载表格类型的文档。

**feishu2md：**

官方地址：[Wsine/feishu2md: 一键命令下载飞书文档为 Markdown (github.com)](https://github.com/Wsine/feishu2md)

我虽然没用实际使用过它，但我阅读它的官方文档后发现它的核心问题是一次只能下载一个文档。

### 我的需求

- 一次导出知识库下的所有文档，包含文档和表格
- 导出的文档目录结构保持和原飞书文档一致
- 导出速度不要太慢
- 对于文档导出的格式没有要求，`docx`和`xlsx`即可

基于以上的种种原因呢，我决定自己动手写一个满足自己需求的程序来解决这个问题。这里我使用的是支持跨平台的.net core进行开发，最终打包程序可支持在`windows`、`linux`、`mac`系统上运行。这里将不赘述具体的实现过程，直接展示最终的效果图吧。

## 如何使用

### 获取AppId和AppSecret

- 进入飞书[开发者后台](https://open.feishu.cn/app)，创建企业自建应用，信息随意填写。进入应用的后台管理页
- （重要）打开权限管理，开通需要的权限：云文档>开通以下权限（注意有分页）
  - 查看新版文档
  - 查看、评论和下载云空间中所有文件
  - 查看、评论和导出文档
  - 查看、评论、编辑和管理云空间中所有文件
  - 查看、评论、编辑和管理多维表格
  - 查看、编辑和管理知识库
  - 查看、评论、编辑和管理电子表格
  - 导出云文档
- 打开添加应用能力，添加机器人
- 版本管理与发布中创建一个版本，并申请发布上线
  - 等待企业管理员审核通过
  - 如果只是为了测试，可以选择测试企业和人员，创建测试企业，绑定应用，切换至测试版本
    - 进入测试企业创建知识库和文档
- 为机器人添加知识库的访问权限，具体步骤如下：
  - 在飞书桌面客户端中创建一个新的群组或直接使用已有的群组
  - 为群组添加群机器人，选择上面步骤中自己创建的应用作为群机器人
  - 打开知识库，如果你是知识库管理员，则可以看见知识空间设置。打开知识空间设置>成员管理>添加成员，选择刚刚建立的群组
- 回到开发者平台，打开凭证与基础信息，获取 `App ID` 和 `App Secret`

### 下载程序

> [v0.0.2.1](https://github.com/xhnbzdl/feishu-doc-export/releases/tag/0.0.2.1)版本为第一个正式发布版本，从`v0.0.2.1`往后迭代的每一个版本都将提供免安装的可执行程序。下载地址：[（Releases）feishu-doc-export](https://github.com/xhnbzdl/feishu-doc-export/releases)

- windows-x64系统，下载`feishu-doc-export-win-x64.zip`

- mac-osx-x64系统，下载`feishu-doc-export-mac-osx-x64.zip`
- linux-x64系统，下载`feishu-doc-export-linux-x64.zip`

下载并解压即可得到程序可执行文件，windows环境的可执行文件为`feishu-doc-export.exe`，`linux`和`mac`环境的可执行文件为`feishu-doc-export`没有后缀。

### 命令行执行

在可执行文件的目录打开终端，命令行所有参数如下：

```
请填写以下所有参数：
  --appId           飞书自建应用的AppId.
  --appSecret       飞书自建应用的AppSecret.
  --spaceId         飞书导出的知识库Id（可为空，或者不传此参数）.
  --exportPath      文档导出的目录位置.
```

- win环境

  ```powershell
  # 指定知识库导出
  ./feishu-doc-export.exe --appId=111111 --appSecret=2222222  --spaceId=333333 --exportPath=E:\temp\测试飞书文档
  # 不指定知识库导出
  ./feishu-doc-export.exe --appId=111111 --appSecret=222222 --exportPath=E:\temp\测试飞书文档
  ```

- linux环境和mac环境

  **注意！！！**首次使用时需要将文件授权为可执行文件

  ```shell
  # 将文件授权为可执行文件
  sudo chmod +x ./feishu-doc-export
  ```

  执行时最好使用`sudo`，否则可能出现权限不足，导致在保存文档时无法创建文件目录

  ```shell
  # 执行不指定知识库的导出
  sudo ./feishu-doc-export --appId=111111 --appSecret=222222 --exportPath=/home/ubuntu/feishu-document
  ```

执行效果图如下：

![image-20230706105636270](https://github.com/xhnbzdl/feishu-doc-export/assets/84184815/aea85f4b-51bc-4e77-a047-1b52b1a75c23)

### 逐步执行

1. 第一步，（win，mac）双击运行程序，输入飞书自建应用的配置，并输入文档要导出的目录位置。

   `mac`和`linux`仍需执行命令`sudo chmod +x ./feishu-doc-export`来将文件设置为可执行文件。

   `mac`可能会出现不受信任的执行程序，需要手动覆盖“隐私与安全性”设置中的设置。`linux`则只能通过命令行输入`.\feishu-doc-export`而不带参数的方式执行

   ![feishuexport_1](https://github.com/xhnbzdl/feishu-doc-export/assets/84184815/cd8b8ab1-ec46-4d19-8844-794e58c305e8)

2. 第二步，选择知识库后自动导出

   ![2](https://github.com/xhnbzdl/feishu-doc-export/assets/84184815/c1a09804-1d9c-414e-94f4-9a5be7230b22)

3. 第三步，对比飞书原文档的目录结构

   ![feishu_wiki](https://github.com/xhnbzdl/feishu-doc-export/assets/84184815/ddc6f0c0-3ace-4498-8bc4-02effc5ee5ea)

## 耗时测试

**700**多个文件导出到本地总耗时**25分钟**

![image](https://github.com/xhnbzdl/feishu-doc-export/assets/84184815/77c97483-8c32-4de0-97d8-a0ef9211cab8)

## 总结

自己动手，丰衣足食，有趣且实用。不过目前我的方案不支持的功能有以下几点，以后有空了再增强

- 不支持将文档导出为`Markdown`格式
- 不支持单独导出一个文档
- 不支持单独导出某个子节点下的所有文档

所以呢，目前我写的这个程序只适用于不要求将文档导出为`Markdown`的群体使用。