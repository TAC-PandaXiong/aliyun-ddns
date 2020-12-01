# aliyun-ddns
 在https://github.com/kaedei/aliyun-ddns-client-csharp的基础上，按照自己的要求，做了一定的修改。
 基于阿里云解析服务API的DDNS客户端。将本机IP更新至指定域名的DNS A记录，配合定时任务可以达到花生壳的效果。

# 使用方法
 aliyun-ddns <accessKeyId> <accessKeySecret> <domainName> <subDomainName> <getIpServer>
  -> accessKeyId    : 例如 *DR2DPjKmg4ww0e79*
  -> accessKeySecret: 例如 *ysHnd1dhWvoOmbdWKx04evlVEdXEW7*
  -> domainName     : 域名，例如 *google.com*
  -> subDomainName  : 子域名，例如 *www*
  -> getIpServer    : 获取外网IP地址的链接

# 获取公网IP的服务
 支持获取IPV4地址的网址列表：
  1.  http://ip.hiyun.me
  2.  http://ip.zxinc.org/getip
  3.  http://v4.ipv6-test.com/api/myip.php
  4.  http://ipv4.icanhazip.com
  5.  http://whatismyip.akamai.com

# 开发环境
 使用VS2019 + C#开发，支持.NET Framework 4.0

# 建议
 建议通过任务计划定时调用（如每小时），程序会判断是否需要修改A记录
