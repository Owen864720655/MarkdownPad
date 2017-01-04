深入分析 Java 中的中文编码问题https://www.ibm.com/developerworks/cn/java/j-lo-chinesecoding/

# File类 #
`file.exists()`判断文件或者文件夹是否存在

`file.mkdirs()`创建文件目录
`file.createNewFile()`创建文件

`file.delete()`删除文件目录或文件，如果文件目录下有子目录，则不允许删除

`file.isDirectory()`判断该File对象是否为目录
`isFile()`判断该File对象是否为文件

**获取File对象的文件/文件目录的路径。**
`getName()`获取文件/文件夹的名字
`getPath()`获取该File对象所代表的文件/文件目录的路径。
`getAbsolutePath()`获取绝对路径
`getCanonicalPath()`获取绝对路径，并且解析.或者..
```java
File file3=new File(".\\test.txt");

System.out.println(file3.getName());
System.out.println(file3.getAbsolutePath());
System.out.println(file3.getPath());
System.out.println(file3.getCanonicalPath());

输出：
test.txt
D:\Users\yth\eclipseworkspace\testIo\.\test.txt
.\test.txt
D:\Users\yth\eclipseworkspace\testIo\test.txt

```

遍历文件夹下所有文件和目录
```java
	public static void getAllFile(File file) throws IOException {
		if (file.exists()) {
			if (file.isDirectory()) {
				if (file.listFiles().length > 0) {
					File[] files = file.listFiles();
					for (File file2 : files) {
						getAllFile(file2);
					}
				} else {
					System.out.println(file.getCanonicalPath());
				}
			} else {
				System.out.println(file.getCanonicalPath());
			}
		}
	}
```

删除一个文件夹及文件夹下面所有的文件
```java
	public static void deleteFile(File file) {
		if (file.isFile()) {
			file.delete();
		} else {
			File[] files = file.listFiles();
			for (File file2 : files) {
				deleteFile(file2);
			}
			file.delete();
		}
	}
```
### 文件过滤器 ###
返回文件夹下所有的子目录/文件list(),listFiles()函数可以有一个参数。

在FileFilter中进行文件过滤
```java
public class MyFileFilter implements FileFilter {
	@Override
	public boolean accept(File pathname) {
		// TODO Auto-generated method stub
		String name=pathname.getName();
		if (name.equals("delete1")) {
			return true;
		}else {
			return false;
		}
	}
}
```
查询文件时
```java
File[]files=file.listFiles(fileFilter);
```