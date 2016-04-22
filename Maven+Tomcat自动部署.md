##给tomcat配置用户名密码##
```xml
<?xml version='1.0' encoding='cp936'?>
<tomcat-users>
<role rolename="manager-gui"/>
  <role rolename="manager-script"/>
  <user username="username" password="password" roles="manager-gui,manager-script"/>
</tomcat-users>
```
##在maven工程的pom.xml中添加如下代码##
```xml
	<build>
		<pluginManagement>
			<plugins>
        <plugin>
            <groupId>org.apache.maven.plugins</groupId>
            <artifactId>maven-compiler-plugin</artifactId>
            <version>2.3.2</version>
            <configuration>
                <source>1.7</source>
                <target>1.7</target>
            </configuration>
        </plugin>
        <plugin>  
            <groupId>org.apache.tomcat.maven</groupId>  
            <artifactId>tomcat7-maven-plugin</artifactId>  
            <version>2.2</version>  
            <configuration>  
                <url>http://localhost:8080/manager/text</url>  
                <username>username</username>  
                <password>password</password>  
                <path>/${project.artifactId}</path>  
            </configuration>  
        </plugin> 

    </plugins>
		</pluginManagement>
		<finalName>maven2</finalName>
	</build>
```
##部署应用##
->Run As->Maven build…->在Goals里面输入tomcat7:deploy
**注意**：部署之前一定要把tomcat打开。