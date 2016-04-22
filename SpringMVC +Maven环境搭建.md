


#SpringMVC+Maven环境搭建#
关于Maven的下载配置就不做介绍。默认Spring插件，maven插件都是安装好的。 
##第一步：新建Maven工程##
![](http://i.imgur.com/vizrcKS.jpg)
![](http://i.imgur.com/zOLmeqz.jpg)
![](http://i.imgur.com/yAamM3f.jpg)
然后ArtifactID就是项目名，这边选SpringMVC
这个时候项目会报错 如下图
![](http://i.imgur.com/MR7F3HN.png)
![](http://i.imgur.com/5s7kfYr.png)
我们在工程目录上点击 Properties -> Java Build Path -> Add Library...-> Server Runtime -> Apache Tomcat -> Finish.就可以解决问题
##第二步：配置pom.xml##

```xml
<project xmlns="http://maven.apache.org/POM/4.0.0" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
  xsi:schemaLocation="http://maven.apache.org/POM/4.0.0 http://maven.apache.org/maven-v4_0_0.xsd">
  <modelVersion>4.0.0</modelVersion>
  <groupId>com.springmvc.handler</groupId>
  <artifactId>SpringMVC</artifactId>
  <packaging>war</packaging>
  <version>0.0.1-SNAPSHOT</version>
  <name>SpringMVC Maven Webapp</name>
  <url>http://maven.apache.org</url>
  <properties>
		<spring.version>4.0.1.RELEASE</spring.version>
	</properties>
	<dependencies>
		<dependency>
			<groupId>junit</groupId>
			<artifactId>junit</artifactId>
			<version>3.8.1</version>
			<scope>test</scope>
		</dependency>
		<!-- Spring dependencies -->
		<dependency>
			<groupId>org.springframework</groupId>
			<artifactId>spring-core</artifactId>
			<version>${spring.version}</version>
		</dependency>

		<dependency>
			<groupId>org.springframework</groupId>
			<artifactId>spring-web</artifactId>
			<version>${spring.version}</version>
		</dependency>

		<dependency>
			<groupId>org.springframework</groupId>
			<artifactId>spring-webmvc</artifactId>
			<version>${spring.version}</version>
		</dependency>

	</dependencies>

  <build>
    <finalName>SpringMVC</finalName>
  </build>
</project>

```
##第三步配置web.xml##
```xml
<?xml version="1.0" encoding="UTF-8"?>
<web-app xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://java.sun.com/xml/ns/javaee" xsi:schemaLocation="http://java.sun.com/xml/ns/javaee http://java.sun.com/xml/ns/javaee/web-app_3_0.xsd" id="WebApp_ID" version="3.0">
  
 
	<display-name>Archetype Created Web Application</display-name>

	<servlet>
		<servlet-name>dispatcher</servlet-name>
		<servlet-class>
			org.springframework.web.servlet.DispatcherServlet
		</servlet-class>
		<load-on-startup>1</load-on-startup>
	</servlet>

	<servlet-mapping>
		<servlet-name>dispatcher</servlet-name>
		<url-pattern>/</url-pattern>
	</servlet-mapping>

	<context-param>
		<param-name>contextConfigLocation</param-name>
		<param-value>/WEB-INF/dispatcher-servlet.xml</param-value>
	</context-param>

	<listener>
		<listener-class>
			org.springframework.web.context.ContextLoaderListener
		</listener-class>
	</listener>
</web-app>

```
##第四步配置dispatcher-servlet.xml   （Spring配置文件）##
dispatcher-servlet.xml在/WEB-INF/dispatcher-servlet.xml目录下面,这个可以在web.xml中进行配置
```xml
<?xml version="1.0" encoding="UTF-8"?>
<beans xmlns="http://www.springframework.org/schema/beans"
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns:context="http://www.springframework.org/schema/context"
	xsi:schemaLocation="http://www.springframework.org/schema/beans http://www.springframework.org/schema/beans/spring-beans-3.2.xsd
		http://www.springframework.org/schema/context http://www.springframework.org/schema/context/spring-context-4.0.xsd">

<context:component-scan base-package="com.springmvc.ythmilk.handler" />

	<bean
		class="org.springframework.web.servlet.view.InternalResourceViewResolver">
		<property name="prefix">
			<value>/WEB-INF/views/</value>
		</property>
		<property name="suffix">
			<value>.jsp</value>
		</property>
	</bean>
</beans>

```
##第五步success.jsp##
这个在/WEB-INF/views/下面。views文件夹需要我们自己新建

```jsp
<%@ page language="java" contentType="text/html; charset=ISO-8859-1"
	pageEncoding="ISO-8859-1"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1">
<title>Insert title here</title>
</head>
<body>
	<center>
		<h2>Success!!!</h2>
		<h2>${message} ${name}</h2>
	</center>
</body>
</html>

```
##第六步Controller##
新建包com.springmvc.ythmilk.handler。在包下面新建类HelloWorld
```java
package com.springmvc.ythmilk.handler;

import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.servlet.ModelAndView;

@Controller
public class HelloWorld {
	@RequestMapping("/hello")
	public ModelAndView showInfo(@RequestParam(value = "name", required = false, defaultValue = "yth") String name) {
		ModelAndView mv = new ModelAndView("success");
		mv.addObject("message", "Welcome to SpringMVC");
		mv.addObject("name", name);
		return mv;
	}
}
```
然后运行：
运行效果
![](http://i.imgur.com/Vv8J3nD.png)
点击之后
![](http://i.imgur.com/qQEk8sj.png)

可以了。还是很简单的。