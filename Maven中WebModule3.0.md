 
默认情况下我们用Module新建的web.xml是2.3版本的，比较旧，我们要改成3.0版本的
web.xml的命名空间声明。
```xml
<?xml version="1.0" encoding="UTF-8"?>
<web-app version="3.0" xmlns="http://java.sun.com/xml/ns/javaee"
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xsi:schemaLocation="http://java.sun.com/xml/ns/javaee http://java.sun.com/xml/ns/javaee/web-app_3_0.xsd">
```
这个时候会报错如下
## `Dynamic Web Module 3.0 requires Java 1.6 or newer` ##
我们需要做一下修改。
1. 默认情况下jdk的版本是1.5我们改为1.7
 ![](http://i.imgur.com/EyuzZr4.png)

2. 右击工程Properties > Java Compiler 然后设置 “Compiler compliance level” 为1.7
 ![](http://i.imgur.com/cXkgQOp.png)
3. 在Navigator视图下面设置.settings文件夹下的内容
    1. **org.eclipse.jdt.core.prefs**设计到1.5的全部改为1.7
	2. **org.eclipse.wst.common.component**将下面的version=”1.5.0”改为version=”1.7.0”
	3. **org.eclipse.wst.common.project.facet.core.xml**将 installed facet=”jst.web” version=”2.3”改为installed facet=”jst.web” version=”3.0”， installed facet=”java” version=”1.5”和 installed facet=”wst.jsdt.web” version=”1.5”中的1.5改成1.7
4. 在pom.xml中添加
```xml
		<plugins>
			<plugin>
				<groupId>org.apache.maven.plugins</groupId>
				<artifactId>maven-compiler-plugin</artifactId>
				<version>3.1</version>
				<configuration>
					<source>1.7</source>
					<target>1.7</target>
				</configuration>
			</plugin>
		</plugins>
```
## Ok烦了我一下午这里面不是每个步骤都是必须的，多试试吧 ##
## 在web.xml中 ##
```xml
<?xml version="1.0" encoding="UTF-8"?>
<web-app xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xmlns="http://java.sun.com/xml/ns/javaee" xmlns:web="http://java.sun.com/xml/ns/javaee/web-app_2_5.xsd"
	xsi:schemaLocation="http://java.sun.com/xml/ns/javaee http://java.sun.com/xml/ns/javaee/web-app_3_0.xsd"
	id="WebApp_ID" version="3.0">
	<display-name>Archetype Created Web Application</display-name>
</web-app>

```