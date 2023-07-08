### 项目打包命令

> `--self-contained`自包含不熟运行时 `-p:PublishSingleFile`单文件部署  `-p:PublishTrimmed`裁剪未使用的库

- win

  ```
  dotnet publish --no-restore -c Release -r win-x64 -o dist/win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true 
  ```

- mac

  ```
  dotnet publish --no-restore -c Release -r osx-x64 -o dist/osx-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true
  ```

- linux

  ```
  dotnet publish --no-restore -c Release -r linux-x64 -o dist/linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true
  ```

### 