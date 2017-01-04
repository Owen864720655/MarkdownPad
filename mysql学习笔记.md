查询中`AS`相当于给力列一个别名
```sql
SELECT id ,name AS county_name FROM tb_county;
```
mysql中常见的约束

| 约束类型      |  标记  |
| -------------|  -----:|
| 主键    | PRIMARY KEY |
| 默认  |UNIQUE |
|外键|FOREIGH KEY|
|非空|NOT NULL|
|自增|AUTO_INCREMENT|

外键约束
```sql
-- 创建市
CREATE TABLE tb_city( 
id INT PRIMARY KEY,
name VARCHAR(20) UNIQUE,
province_id INT NOT NULL,
CONSTRAINT fk_province FOREIGN KEY(province_id) REFERENCES tb_province(id)
);

```
CONSTRAINT 约束名 FOREIGN KEY(要依赖属性) REFERENCES 依赖的表(依赖的属性)

## 查询 ##
查询中的一些语法`BETWEEN AND`和 `NOT BETWEEN AND`进行范围查找
`IS NULL`判断字段数据是否为空
`IS NOT NULL`判断字段不为空
```sql
WHERE field IN(value1,value2,value3...)--是否在指定的集合中查找
NOT IN --不在集合中
```

排序 `ORDER BY` `ASC`升序，`DESC`降序

限制数据记录查询数量
`LIMIT OFFSET_START,ROW_COUNT`

带`LIKE`关键字的模糊查询` WHERE filed LIKE value`
"_"通配符，匹配一个字符
 "%"能匹配任意长度的字符

分组查询 `GROUP BY` 现将所有数据进行分组，然后显示每组中第一条数据
`HAVING value`对组进行限制

----------

## 多表查询 ##
**外连接：**在表关系的笛卡尔积中，不仅保留表关系中匹配的，还包括不匹配的数据记录。(先笛卡尔积，然后根据查询条件进行删选)
- 左外连接  `LEFT JOIN /LEFT OUTER JOIN `
- 右外连接  `RIGHT JOIN/RIGHT OUTER JOIN`
- 全外连接  `FULL JOIN/FULL OUTER JOIN`

```sql
-- 左连接
SELECT * FROM student LEFT JOIN course ON student.Id=course.id
-- 右连接
SELECT *FROM  student RIGHT JOIN course ON student.id=course.id;
--全连接(版本不支持 ，勿用)
SELECT *FROM student FULL JOIN course ON student.id=course.id; 
```

**内连接**`INNER JOIN`
```sql
SELECT *FROM student INNER JOIN course ON student.id=course.id; 
```

多对多查询(两次左外连接)
```sql
select s.name,c.c_name from student 
      as s left join student_course 
          as sc on s.id=sc.Sno 
             left join course as c 
                on c.id=sc.Cno;
                
                
SELECT student.name ,course.c_name FROM
   student LEFT JOIN student_course  ON student.id=student_course.sno 
       LEFT JOIN course ON course.id=student_course.cno;
```

创建存储过程 插入数据成功返回1，失败返回0
```sql
USE mymvc;
DROP procedure if exists insertTest;
delimiter //
create procedure insertTest(in name varchar(10),in salary double ,out row int)
begin
insert into s_user (user_name,user_salary)values(name,salary);
if (row_count()>0) then set row=1;
else set row=0;
end if;
select row;
end //
delimiter ;
call insertTest('测试',100,@row);
```

```sql
USE mymvc;
DROP procedure if exists insertTest;
delimiter //
create procedure insertTest(in name varchar(10),in salary double )
begin
declare row int;
insert into s_user (user_name,user_salary)values(name,salary);
if (row_count()>0) then set row=1;
else set row=0;
end if;
select row;
end //
delimiter ;



/* 查询所有的*/
DROP PROCEDURE IF exists searchAll;
DELIMITER //
CREATE PROCEDURE searchAll()
begin 
SELECT *FROM s_user;
end //
DELIMITER ;
```

测试函数
```sql
use mymvc;
DELIMITER $$
CREATE FUNCTION TESTFUN(ID INT)
RETURNS VARCHAR(30)
BEGIN
DECLARE name VARCHAR(30);
SET NAME=NULL;
SET NAME=(select user_name from s_user where s_user.user_id=id);
return name;
END $$

DELIMITER ;
SELECT TESTFUN(3)
```

创建临时表：http://blog.163.com/duanpeng3@126/blog/static/8854373520104614056798/

游标写法 http://blog.csdn.net/anialy/article/details/8106370

游标
```sql
USE mymvc;
DROP procedure if exists insertTest;
delimiter //
create procedure insertTest( )
begin
  declare done int default -1;  
DECLARE TMPCODE INT;
DECLARE MY_CURSOR CURSOR FOR SELECT user_id FROM s_user;
declare continue handler for not found set done=1;  
#打开游标

# 不存在则创建临时表  
 CREATE TEMPORARY TABLE IF NOT EXISTS EquipmentSTemp (
 ID int
 );
 truncate TABLE EquipmentSTemp;#使用之前清空
 OPEN MY_CURSOR;
    /* 循环开始 */  
    myLoop: LOOP 
 
 
FETCH MY_CURSOR INTO TMPCODE;

 if done = 1 then   
        leave myLoop;  
        end if;  
			INSERT INTO EquipmentSTemp VALUES (TMPCODE); #根据设备状态码获取相应的ID		END	IF;
end loop myLoop;  
CLOSE MY_CURSOR;
SELECT 
    ID
FROM
    EquipmentSTemp
ORDER BY ID DESC;
end //
delimiter ;

call insertTest();
```

游标的使用http://blog.csdn.net/liguo9860/article/details/50848216
临时表
```sql
 CREATE TEMPORARY TABLE IF NOT EXISTS EquipmentSTemp (
 ID VARCHAR(10)
 );
 truncate TABLE EquipmentSTemp;#使用之前清空
```
```SQL
CREATE TEMPORARY TABLE tmp_table SELECT * FROM table_name
```

存储过程调用函数
```sql

#测试一个表的更新语句对另一个表的影响。
use mymvc;
drop function if exists UPTEST;
DELIMITER $$
CREATE FUNCTION UPTEST(ID INT)
RETURNS DOUBLE
BEGIN 
declare salary double;
UPDATE MYMVC.s_user SET s_user.user_salary=user_salary+1 WHERE S_USER.user_id=ID;
set salary=(select user_salary from s_user where s_user.user_id=id);
return salary;
END $$
DELIMITER ;
select UPTEST(3);
select user_salary from s_user where s_user.user_id=3;

drop procedure if exists haha;
delimiter //
create procedure haha(in id int)
begin
declare salary double ;
declare row int;
set salary=UPTEST(id);
if (row_count()>0) then set row=1;
else set row=0;
end if;
select row;
end //
delimiter ;

call haha(3);
```

事务
```sql
start transaction;
rollback;
COMMIT;

DROP PROCEDURE IF EXISTS InsertGroup;
DELIMITER //
CREATE PROCEDURE InsertGroup(
IN Name nvarchar(100), 
 IN Memo nvarchar(200) )
 
BEGIN 
DECLARE TMPID VARCHAR(10);
	DECLARE ROW INT;
    DECLARE CURRENTNUM VARCHAR(10);
    DECLARE ERROR INT;
	SET TMPID=(SELECT  ID FROM TB_Group WHERE TB_Group.Name=Name AND TB_Group.Valid=1);   
  IF (TMPID IS NOT NULL) THEN SET ROW=0;
  ELSE 
  BEGIN 
  start transaction;
  set CURRENTNUM=GetSerialNumber('群组');
  IF (row_count()>0) THEN SET ERROR=(ERROR+1);
  END IF;
  INSERT INTO TB_Group VALUES (CURRENTNUM,Name,
		Memo,1);
   IF(row_count()>0) THEN  SET ERROR=(ERROR+1);
   END IF;
   
   IF (ERROR>0)then 		
			rollback; #如果发生错误进行回滚
			set row= -1;	
        ELSE		
			COMMIT;
			set row= 1;
   END IF;
IF(row_count()>0) THEN SET ROW=1;
ELSE SET ROW=0;
END IF;
END;
END IF;
END //
DELIMITER ;


```



判断数据库中是否存在某个表
```sql
IF EXISTS (SELECT *FROM information_schema.TABLES WHERE table_name =(Table_Name)
```


存储过程中动态执行sql语句
```sql
delimiter //
create procedure ma()
begin 
declare str varchar(2000);
set str=('DROP TABLE tb_smsalarmmode');
set @dropStr=str;
prepare stm from @dropStr;  
execute stm;
end //
```


时间格式化 和 求时间差
```sql
set dd= (date_format('2016-12-22 15:37:00 ','%Y-%m-%d %T'));
Set isValid=(select TIMESTAMPDIFF(minute,dd,now()));
```
mysql 中的try catch
```sql
DECLARE CONTINUE HANDLER FOR SQLEXCEPTION,SQLWARNING,NOT FOUND set _err=1;
```
 作用是当遇到SQLEXCEPTION,SQLWARNING,NOT FOUND 错误时，设置_err=1并执行CONTINUE操作，即继续执行后面的语句。


注意:
function 中的增删改 可以在存储过程中通过　row_count()捕获，而存储过程的不行。
function 中和触发器中不能有 commit等事务操作。