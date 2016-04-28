# Maven依赖记录 #
##Spring##
1. 最基本的Spring 依赖
```xml
		<dependency>
			<groupId>org.springframework</groupId>
			<artifactId>spring-context</artifactId>
			<version>4.2.5.RELEASE</version>
		</dependency>
```
2. Spring Persistence与Maven
它提供了Hibernate和JPA支持,如 HibernateTemplate 和 JpaTemplate —— 以及持久性相关的一些依赖关系: spring-jdbc 和 spring-tx.
JDBC Data Access库定义了 Spring JDBC支持 以及 JdbcTemplate, 而 spring-tx 代表了非常灵活的 事务管理的抽象(Transaction Management Abstraction).
```xml
		<dependency>
			<groupId>org.springframework</groupId>
			<artifactId>spring-orm</artifactId>
			<version>4.2.5.RELEASE</version>
		</dependency>
```

3.  Spring MVC与Maven
spring-web 依赖包含Servlet和Portlet环境中常用的web特定工具,而 spring-webmvc 对Servlet环境提供了MVC支持.
```xml
		<dependency>
			<groupId>org.springframework</groupId>
			<artifactId>spring-web</artifactId>
			<version>4.2.5.RELEASE</version>
		</dependency>
		<dependency>
			<groupId>org.springframework</groupId>
			<artifactId>spring-webmvc</artifactId>
			<version>4.2.5.RELEASE</version>
		</dependency>
```
Spiring需要注意的地方：
1. 使用里程碑版本(Milestones)
需要在repositories指定Spring自己的Maven仓库
```xml
<dependencies>
    <dependency>
        <groupId>org.springframework</groupId>
        <artifactId>spring-context</artifactId>
        <version>4.3.0.RC1</version>
    </dependency>
</dependencies>
<repositories>
    <repository>
        <id>spring-milestones</id>
        <name>Spring Milestones</name>
        <url>https://repo.spring.io/libs-milestone</url>
        <snapshots>
            <enabled>false</enabled>
        </snapshots>
    </repository>
</repositories>
```
2. 同理还有快照版本（Snapshots）
```xml
<dependency>  
    <groupId>org.springframework</groupId>  
    <artifactId>spring-core</artifactId>  
    <version>4.0.3.BUILD-SNAPSHOT</version>  
</dependency>
<repositories>  
    <repository>  
        <id>repository.springframework.maven.snapshot</id>  
        <name>Spring Framework Maven Snapshot Repository</name>  
        <url>http://repo.spring.io/snapshot/</url>  
    </repository>  
</repositories>  
```
##数据库操作##
1. Hibernate
```xml
<dependency>
	<groupId>org.hibernate</groupId>
	<artifactId>hibernate-core</artifactId>
	<version>5.1.0.Final</version>
</dependency>
```
2. MySql连接
```xml
<dependency>
			<groupId>mysql</groupId>
			<artifactId>mysql-connector-java</artifactId>
			<version>6.0.2</version>
</dependency>
```
3. c3p0连接池
```xml
<dependency>
	<groupId>c3p0</groupId>
	<artifactId>c3p0</artifactId>
	<version>0.9.1.2</version>
</dependency>
```