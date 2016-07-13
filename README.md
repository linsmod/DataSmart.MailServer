# DataSmart.MailServer

.Net Mail Server based on Lumisoft opensource project

<img src="http://www.dsvisual.cn/Media/Default/dsmailserver/dsmailserver.png"></img>

# Features

- supports pop3 smtp imap etc.
- supports ssl/tls for pop3, smtp and imap.
- supports multi mail vitural server, can bind multi domains.
- supports run as windows service, winform application with or without tray icon control.
- supports remote management using mailservermanager.exe
- supports mail recycle bin
- supports plugins using api interfaces

# Build and release

- download source code or use the releases i provided.
- if from source code you should run afterbuild.bat after build the solution successfuly
- then you get the debug or release version from application folder.

# Installation

run MailServerLauncher.exe to install as windows service or just run as desktop application with or without tray icon.

# Configuration

run MailServerManager.exe at the machine runs mailserver service or app.

## Connect to server
- press connect button from menu.
- type server localhost or 127.0.0.1 or leave it empty
- type username Administrator with case sensitive
- type password emtpy
- press ok to connect with saving or not

## Add virtual server

- type name and select storage api
- [your vitural server]>system>general, add dns servers for query mailto's domain mx record.
- [your vitural server]>system>services, enable smtp and pop3 services and set ipaddress binding with or without ssl/tls. the host name is required when set bindings. eg. bind smtp service to smtp.[your.domain] + IPAddress.Any
- [your vitural server]>system>service>relay, 'send email using DNS' and set at least a local ipaddress binding for email sending. the name of the binding here only a name,not mean domain.
- [your vitural server]>system>logging, enable logging for all services when something error you can see the details from 'Logs and Events' node
- [your vitural server]>domains, set email host domain, eg. if your email will be xyz@abc.com then the domain should be abc.domain, description is optional
- [your vitural server]>security, !!! important, add rules for your service to allow outside access like email client.

eg. add a rule 'Smtp Allow All' for smtp service with ip allows between 0.0.0.0 to 255.255.255.255 to enable smtp service for outside access

add 'Pop3 Allow All' and 'Rlay Allow All'  like that too.
- [your vitural server]>filters, there are two types of filter named 'DnsBlackList' and 'VirusScan' its configurable by run it's executable from mail server install path.

## Domain name resolution
- Add smtp.[your.domain], pop3.[your.domain], imap.[your.domain] resolution to your server public ip address with A record or to your server domain with CNAME record.
- mx record is optional if [your.domain] has a A record.if not, you shoud add a mx record point to your server ip.

# Remote management
- to enable remote management you must add ACL to allow mail server managers connect from outside network.
- use MailServerAccessManager.exe to add management users or just use administrator.
- add rules to allow access from specific IPs or ip ranges
- The users here only for management cases.

# 邮件服务器软件

源码托管于：https://github.com/kissstudio/DataSmart.MailServer

使用.Net开发的邮件服务器，基于Lumisoft开源项目构建，包含以下特性

- 支持POP3，IMAP，SMTP
- SSL/TLS 加持
- 支持在同一个服务端配置多个虚拟服务器
- 支持绑定多个不同的域名
- 支持以 Windows服务 或者 Winform应用程序 运行，可以通过任务栏图标启动管理器
- 支持通过邮件服务器管理器程序（MailServerManager.exe）进行远程管理和配置
- 在远程管理功能上，支持基于IP和账号的安全性访问控制
- 支持邮件回收站
- 支持基于接口的二次开发
- 提供付费技术支持，如有需要请联系我
- 




