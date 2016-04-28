#servlet中的相对路径和绝对路径#
web站点目录：http://localhost:8080/
web应用程序的根目录: http://localhost:8080/test/

##web.xml中的路径##
Servlet 映射 `<url-pattern>/xx</url-pattern>` 中的 “/” 代表当前 web 应用的根路径。
所有的web.xml文件都是描述某个web应用的部署相关信息，所以“/”只能代表当前 web 应用的根路径，而不是指向web站点的根目录。
##转发和重定向中的路径问题##
1. 如果没有“/”,如request.getRequestDispatcher("hello.jsp ").forward(request, response) 和response.sendRedirect(" hello.jsp ") 就表示在同级目录中寻找login.jsp文件。
2. 如果路径中包含“/”（注意，这里的“/”是指路径中的第一个“/”）
	1.  转发
	如request.getRequestDispatcher("/hello.jsp").forward(request, response) ：代表到http://localhost:8080/test/ 下目录寻找hello.jsp文件。(即web应用的根目录)

	2. 重定向
	如response.sendRedirect("/hello.jsp")：代表到http://localhost:8080/ 目录下寻找hello.jsp文件(即web站点的根目录)

##`<form action=“/xxx”> 或 <a href=“/xxx”></a>`中的路径##
“/”代表的是 web 站点的根路径。
因为超链接可以链接到任何需要的目标资源, 所以 / 代表的肯定不是当前 web 应用的根路径, 而是当前 web 站点的根路径。
如果不加"/"的话是默认当前页面所在的路径、

 `index.jsp`默认实在应用根目录上，Servlet也是，因为在web.xml进行了映射，web.xml就是依据web根目录的.

**解释：**首先必须理解重定向的概念：重定向其实向服务器发送了2次请求，第一次把请求url告诉给服务器，服务器看到这个请求的状态码，马上明白该url是需要浏览器来重新去请求的url，于是呢把该url显示在浏览器的地址栏内，再次向服务器请求（第二次了吧），服务器响应，然后返回。

很明显，只要和页面打交道的，"/"都代表站点名，需要在后面添加应用名以区分是哪个应用的请求。
(比如表单的`action`,比如连接的地址，"/"都是相对于web站点的根目录，不加/的话是相对当前页面的目录).