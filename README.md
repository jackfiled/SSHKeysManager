# SSHKeysManager

北邮人团队技术组2022年考核实践项目——SSH公钥授权系统

## 设计原理

在`SSH`服务端的配置文件中存在一项`AuthorizedKeysCommand`配置，这项配置可以指定一个脚本文件查找用户的公钥作为认证，在用户尝试登录时系统会调用这个脚本，可选择的参数有用户尝试登陆时使用的私钥对应的公钥、指纹和登录用户等的变量，输出要求是0行或者多行`authorized_keys`格式的公钥列表。

本系统就围绕着这个配置选项设计。在用户尝试登录服务器之时，`SSH`服务端会优先在`authorized_keys`文件中查找用户的公钥文件，如果在文件中没法找到对应的公钥才会运行上述的脚本，这样就不会对原本采用`authorized_keys`文件管理授权的用户产生影响。在运行脚本时，脚本会给认证服务器发起一个`HTTP GET`请求，认证服务器在验证服务器的身份之后，按照`authorized_keys`文件的格式返回有权访问这台服务器的用户的公钥列表。

```
❯ curl http://localhost:5155/serverLogin?token=12345678
ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQC9IL7tvAXT/uEWJnhsEh2uNtA/6hllAR6+iEXpVvbC9YkdgRxJo/Fqdm8AoJ1x5T49JC1tPcl1y68a9Sp+cDullOredvZYsfEuBDKyHmE+5EoLhv8wPuh5RHM3d2UrxqqDxs9Bi3bcLuUku063MU6VDK4HN1+d6DYmvnSxUWxkuRYXu9hlmoaToC3ayFIkaZMXuc4GuFDvKln4lxy90U/SVgBstbYr+vVb57kCsgGM0jQ5gYPF/anST9KDPiUCR5xaB01bqkL9GUaJfR1irNQfzOgbnByM19stxA6zqS9smfUi71SMT4rnbC7x3OjY99gGnLnWRvUJBhIQsFyaMosJ r.ofcdunj@qq.com
ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQCfMnb4XKY5k5iAiOtQtItSixe8tMbtYlUanEtUjuE3bk0QwG6ADh9HI8MS4vHuQCXmz8GpgaA87LO3by0MNW+eWQhBxr7viCak3uEenGnd0GoucZuQ5eB4R7fCook3PNSgHjNRlVlBBHkd2GOlwXClzR9A+wIWvECM4ky7MVA6j9JNUejZ8W/Wwt+9KNjHL1H2kGjLesh3TueM5Me50rR7RKR/4D3ZOy7SVUPTVNbScaGtXc+wcB5QGWp7T7aWbf7Z31a8B5UoN6lsq3MG4skC8GXtdrLl13RCZ4UNDNTPDmL7EP3/EJAbIDUheHN90f44WNWZq50pJPGYVNmexIxh r.ofcdunj@qq.com
ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQC4Hy749fqJIIqHnJCHSLPvUh3e53JliLRozF3hFGYAGVDLXLJ+Ix4mKCCql402MAn4ExcPstddGRFQgyylvLg2mAbhERz3z4iCTRkvg1VOBcUO7Jeech5WE7fyRNQolzWO32UiDxfDb9Bu55OGS3Dx9PHBVKEoDQLXYrVMbyzrRV8YGrwLrAJ1EREK4ukdR4X6XLD8XISqNXqrbjSL2T8IDVFOud8Svxw+eQ2V4kxIMbJz+/2QwDfF9WHVA+yoCsf3CI6RFR/LHgWcdi1VBxhSZgx/HX7JiZKkvO2rwQKfDERS48C2IeNKjsy+cEgcbENllO+oM6abKke/24o/HQXt r.ofcdunj@qq.com
ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQDDRkRx8iHQtctUAQT4wUhWW9C+bm6lOGxR1hmm39ZQRkYSQ04nXolGAvQY2OgcXLq9S5RAEzhx4Yr05679MvQfiAIfvuTVNjU/UzHtVwrCPSlrhDEUKPeGIzIcNUSgs5z3+b5O2iie5rLw+t172DZP3Mh7GTDGXK4d0pEq5jam8mRR4oRWZ9H12GRVOW9uHrVM5jSFiIz88lkYzYrbtMkttiu9Yq9vlHxSmxY1cblHgWho5fj1Vkyf7rrMDnlwnXK1eRb/BMdAhAj2SzDq/ly+F1lNSQPSvqcknxIzwENFXfuxJe0t56pkXPq4my2n5/RCgnyY28Fhvv8nqrJMKPhV r.ofcdunj@qq.com
ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQCsaWVkexMdb2Ze9+w/TN3pREVpYaANkcbHjOvXSBnD0y2H12RhL+tIqXga5y8NSJ5YLsYovIgtsSTD4gCfVQ1CCyKWpb6TwVgmHXa8m+pYHOG6twKgtR5V+y1ISqlr7md9OcgeyBGkVNaghR+ddaOPhWJmOzdN4ZW5BCH/zYukUdbtM6/0SmAkMYsQxdBojPQMS16gejkoL0ixfZgFWQMIi4K9IH3dK3kmcc/ruHf5FfNbybbUsym/Afo5zX532e2eAJFCOKNtLtai8xDEqo0JWYOzzc4ix3KAOK4mJZcZxtp13k1NH3pRlb6kkiz+rfm/D88854znEE6Xs8jxSd1R r.ofcdunj@qq.com
ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQDYVQycVTDPuTT2QFVMJs3e1kWihI1TlwjO09xEqwDQWAGG1uGFonwfMRbv3XpJ9lIK+WvolRjT0a3DOvma6xjKvfdU3O/cbX2VFOCGNnl6f8ElAr5KXvbTTaJ3B80vJIR+zXBQbFs7cUdcC50ZxCcgjuzWCcqxSqesX3v4AQCxWdSnD2MtSTewhiEYZooSeb7Ek6AAamCqAjWxmqkqCkZl/Rmza6Q9iOB1C8ku/HSYdZYTjnMc6fQESSWNY1gES6cT0P6hBE/qtbBik+yB8Q8ZNW3B3qwknDlg3VLYjYHruHPkqUxzfq4aIRKihz5rC3FMOML/rw4X1v9/gXF5Aa3x r.ofcdunj@qq.com
```



## 实现功能

### API

#### 基础功能

- 用户登录、注册、修改密码
- 用户添加公钥、删除公钥、生成公钥
- 服务器请求生成授权公钥的列表

#### 管理功能

- 添加、删除、修改用户
- 添加、删除服务器
- 编辑服务器和用户之间的对应关系

> 具体API详情请见仓库[WiKi](https://github.com/jackfiled/SSHKeysManager/wiki/API-Docs)

### 管理页面

显然的，一个简介优雅的前端页面会大大提升用户的使用体验，即使是一个粗糙简陋的命令程序封装也比直接手搓`curl`语句访问管理API更加人性化。但是，我们不得不承认的是，鉴于设计编写前端页面在程序本身上的复杂和某些开发人员在技能缺失上一些不得不称之为巧合的矛盾，再加之后端开发过程由于开发人员意图在主流开发框架和个人技能成长上达成一个比较完美的平衡，抓住时机积极锻炼开发人员的敏捷开发技能，我们可能不得不对于前端页面的前景提出一个不可称为乐观的预计。

## 安装

项目依赖于`.NET`框架和`ASP.NET Core`框架，请在[这里](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)安装。如果需要编译运行代码请下载安装`.NET SDK`，如果仅需要运行，安装`ASP.NET Core Runtime`就可以了。

安装完成之后，使用`dotnet SSHKeysMangager.dll`命令即可运行服务，程序默认会在`localhost`的`5155`端口上监听。

### 配置选项

程序的配置文件为文件夹下的`appsettings.json`文件。

- 修改`Kestrel-Endpoints-Http-Url`项可以修改程序运行时监听的端口

### 数据库的初始化

在程序运行目录下创建名为`SSHKeysManager.db`的SQLite数据库文件，执行`database.sql`文件在数据库中创建程序中会用到的表。

每次程序运行时，都会检查数据中是否存在名为`admin`的管理员，如果不存在，则会创建名为`admin`，电子邮件地址为`admin@admin.com`，密码为`admin`的初始管理员。

## 开源协议

MIT
