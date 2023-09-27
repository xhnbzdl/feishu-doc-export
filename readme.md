# feishu-doc-export
一个支持Windows、Mac、Linux系统的飞书文档一键导出服务，仅需一行命令即可将飞书知识库的全部文档同步到本地电脑。支持导出`markdown`，`docx`，`pdf`三种格式。导出速度嘎嘎快，实测**700**多个文档导出只需**25**分钟，且程序是后台挂机运行，不影响正常工作。最新版本内容，请查看文章最后的**更新日志**

## 动机

最近也是公司办公软件从飞书切换回了企业微信，自然就产生了一些文档要迁移的问题，由于文档量过多（大概有700多个），无论是从飞书手动下载为`Word`或`PDF`格式的文档，还是将内容复制到本地新建`Markdown`文件都是一件极为繁琐的事情。于是便找到了两个GitHub上已有的飞书文档导出工具`Feishu2MD`和`feishu-backup`，但是他们都有一些问题不太满足我的需求。

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
  - 打开知识库，如果你是知识库管理员，则可以看见知识空间设置。打开知识空间设置>成员管理>添加管理员，选择刚刚建立的群组
- 回到开发者平台，打开凭证与基础信息，获取 `App ID` 和 `App Secret`

### 下载程序

> 下载地址：[（Releases）feishu-doc-export](https://github.com/xhnbzdl/feishu-doc-export/releases)，请选择最新版本下载

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
  --type            知识库（wiki）或个人空间云文档（cloudDoc）（可选值：cloudDoc、wiki，为空则默认为wiki）.
  --saveType        文档导出的文件类型（可选值：docx、md、pdf，为空或其他非可选值则默认为docx）.
  --folderToken     当type为个人空间云文档时，该项必填.
```

- win环境

  ```powershell
  # 指定知识库导出
  ./feishu-doc-export.exe --appId=111111 --appSecret=2222222  --spaceId=333333 --exportPath=E:\temp\test
  # 不指定知识库导出
  ./feishu-doc-export.exe --appId=111111 --appSecret=222222 --exportPath=E:\temp\test
  # win 不指定知识库 将文档保存为markdown文档
  ./feishu-doc-export.exe --appId=xxx --appSecret=xxx --saveType=md --exportPath=E:\temp\test
  # win 导出个人空间文档 将文档保存为markdown文档
  ./feishu-doc-export.exe --appId=xxx --appSecret=xxx --saveType=md --exportPath=E:\temp\test --type=cloudDoc --folderToken=xxx
  ```

- Linux环境和mac环境

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

## 个人空间文档导出

操作步骤，请参考**更新日志**[feishu-doc-export-v 0.0.4 ](https://github.com/xhnbzdl/feishu-doc-export/releases/tag/0.0.4)

## 耗时测试

**700**多个文件导出到本地总耗时**25分钟**

![image](https://github.com/xhnbzdl/feishu-doc-export/assets/84184815/77c97483-8c32-4de0-97d8-a0ef9211cab8)

## 更新日志

### 2023-7-15发布[feishu-doc-export-v 0.0.3 ](https://github.com/xhnbzdl/feishu-doc-export/releases/tag/0.0.3)

- 这个版本新增了两种格式的导出，可支持将飞书文档导出为`markdown`和`pdf`，加上原有支持的`docx`一共是三种格式。

- 新增了命令行参数`--saveType`，文档保存的格式类型，可选值有`md`，`pdf`，`docx`，如果参数不传，或值为空，或值为不存在的格式，则默认导出为`docx`。使用方式如下：

  ```shell
  # win 不指定知识库 将文档保存为markdown文档
  ./feishu-doc-export.exe --appId=xxx --appSecret=xxx --saveType=md --exportPath=E:\temp\test
  
  # mac 不指定知识库 将文档保存为pdf
  sudo ./feishu-doc-export --appId=xxx --appSecret=xxx  --exportPath=/home/feishu-document --saveType=pdf
  
  # linux 不指定知识库 将文档保存为docx
  sudo ./feishu-doc-export --appId=xxx --appSecret=xxx  --exportPath=/home/feishu-document 
  sudo ./feishu-doc-export --appId=xxx --appSecret=xxx  --exportPath=/home/feishu-document --saveType=
  sudo ./feishu-doc-export --appId=xxx --appSecret=xxx  --exportPath=/home/feishu-document --saveType=docx
  sudo ./feishu-doc-export --appId=xxx --appSecret=xxx  --exportPath=/home/feishu-document --saveType=abcdefg
  ```

- 耗时测试

  - 导出为`docx`最快
  - 导出为`markdown`和`docx`的速度差不多
  - 导出为`pdf`速度最慢，因为`pdf`的图片是内嵌的
  - 实际速度与网速和飞书服务器响应，电脑磁盘写入速度都有关系

- 注意事项：

  1. 文档导出为`markdown`时，存在文档格式丢失的问题，原因是因为我的实现方式是利用飞书自提供的接口先将文档下载为`docx`，然后再将`docx`转为`markdown`，文档下载为`docx`后就已经存在格式丢失的问题了，所以不能很好的转换为`markdown`。而上面提到的两个开源库都是自己做的处理，它们都是直接将飞书原始数据转换为`markdown`语法的。`feishu-backup`是作者自己对飞书原始数据做的转换（牛逼），`feishu2md`则是用了一个针对飞书数据转换的库。

  2. `feishu-doc-export`目前已发现`docx`转为`markdown`丢失的格式有：引用语法、表格、行内代码块

  3. 对于飞书文档中引用的其他文档，如果引用的文档是当前知识库的文档，则该文档下载到本地后会以相对路径引用另一个文档，因为另一个文档也会下载到本地。

     如果引用的文档是其他知识库或者是外链，则当前文档下载后还是以原文方式引用。

- 导出的效果图展示，由于图片大小原因请移步[效果图查看链接](https://www.cnblogs.com/hyx1229/p/17533325.html#%E6%9B%B4%E6%96%B0%E6%97%A5%E5%BF%97)

### 2023-9-27发布[feishu-doc-export-v 0.0.4 ](https://github.com/xhnbzdl/feishu-doc-export/releases/tag/0.0.4)

- 支持导出知识库内的文件类型文档，如：pdf、image等。

- 支持个人空间云文档导出（需要指定文件夹的Token）

- 优化了程序异常处理，保证下载尽可能不中断

- 新增了命令行参数`--type`和`--folderToken`，选择导出知识库或个人空间云文档，可选值：`cloudDoc`、`wiki`，为空则默认为`wiki`。当`type=cloudDoc`时，需要填写`--folderToken`参数，`type=wiki`或空，则不需要填写。使用方式如下：

  ```powershell
  # win 导出个人空间文档 将文档保存为markdown文档
  ./feishu-doc-export.exe --appId=xxx --appSecret=xxx --saveType=md --exportPath=E:\temp\test --type=cloudDoc --folderToken=xxx
  ```

- 如何导出个人空间的文档

  1. 将要导出的文件夹分享给自建应用，让自建应用拥有可导出文档的权限。

    ![image-20230927162804954](https://github.com/xhnbzdl/feishu-doc-export/assets/84184815/668c5d5a-9b6e-4410-9511-a5027167eb6b)

  2. 获取`folderToken`：

  
   ![image-20230927161804968](https://github.com/xhnbzdl/feishu-doc-export/assets/84184815/b094b108-7ff4-4b2f-860a-b5b8298b1e15)


  3. 执行命令：

     ![image-20230927163239528](https://github.com/xhnbzdl/feishu-doc-export/assets/84184815/1d049f23-ad39-4260-a8fe-e4214b87c953)

- 为什么不支持列举文件夹列表？

  因为飞书对于个人空间做了登录限制，未登录情况下个人空间相关的部分`API`无法直接调用。而要支持登录，飞书只提供了网页端的接入方案，因此该项目不支持。