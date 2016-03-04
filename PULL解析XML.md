# PULL解析XML文件 #

----------
常用的XML pull的接口和类：

- XmlPullParser：该解析器是一个在org.xmlpull.v1中定义的解析功能的接口。

- XmlSerializer：它是一个接口，定义了XML信息集的序列。

- XmlPullParserFactory：这个类用于在XMPULL V1 API中创建XML Pull解析器。

- XmlPullParserException：抛出单一的XML pull解析器相关的错误。

PULL解析器的运行方式和SAX类似，都是基于事件的模式。
## 标签说明 ##

<table class="table table-bordered table-striped table-condensed">
<tr>
<td>标签名</td>
<td>说明</td>
<td>对应的数字</td>
</tr>
<tr>
<td>START_DOCUMENT</td>
<td>文档开始标记</td>
<td>0</td>
</tr>
<tr>
<td>END_DOCUMENT</td>
<td>文档结束标记</td>
<td>1</td>
</tr>
<tr>
<td>START_TAG</td>
<td>标签开始标记</td>
<td>2</td>
</tr>
<tr>
<td>END_TAG</td>
<td>标签结束标记</td>
<td>3</td>
</tr>
</table>
## 实例 ##
初始化处理
```java
//从assets中读取xml文件
InputStream is=getResources().getAssets().open("test.xml");
//创建XmlPullParser实例
XmlPullParser parser =Xml.newPullParser();
//设置输入流，并指明字符编码
parser.setInput(is,"utf-8");
//产生第一个事件(即标签对应的Int类型)
int evenType=parser.getEventType();
```
具体解析
```java
parser.getName()//获取标签名
parser.getText()//获取标签值
parser.getAttributeName(0);//获取第一个属性名
parser.getAttributeValue(0)//获取第一个属性值
evenType=parser.next();//获取下一个标签
```

```java

 while (evenType!=XmlPullParser.END_DOCUMENT){
                        switch (evenType){
							//文档开始事件,可以进行数据初始化处理
							case XmlPullParser.START_DOCUMENT:
							break;
                            case XmlPullParser.START_TAG:
                                if (parser.getName().equals("person")){
                                     person=new Person();
                                }else if (parser.getName().equals("name")){
                                     evenType=parser.next();
                                    person.setName(parser.getText());
                                }else if (parser.getName().equals("age")){
                                    evenType=parser.next();
                                    person.setAge(parser.getText());
                                }else if (parser.getName().equals("sex")){
                                    evenType=parser.next();
                                    person.setSex(parser.getText());
                                }
                                break;
                            case XmlPullParser.END_TAG:
                                if (parser.getName().equals("person")){
                                    personList.add(person);
                                    person = null;
                                }
                                break;
                        }
                      evenType = parser.next();
                    }
                } catch (IOException e) {
                    e.printStackTrace();
                } catch (XmlPullParserException e) {
                    e.printStackTrace();
                }
 				String info="";
                for (Person p:personList){
                    Log.i("personInfo",p.toString());
                    info+=p.toString();

                }
```
## XML生成 ##
```java
 public String serialize(List<Book> books) throws Exception {
 
        // 由android.util.Xml创建一个XmlSerializer实例
        XmlSerializer serializer = Xml.newSerializer();
        StringWriter writer = new StringWriter();
        // 设置输出方向为writer
        serializer.setOutput(writer);
        serializer.startDocument("UTF-8", true);
        serializer.startTag("", "books");
 
        for (Book book : books) {
            serializer.startTag("", "book");
            serializer.attribute("", "id", book.getId() + "");
 
            serializer.startTag("", "name");
            serializer.text(book.getName());
            serializer.endTag("", "name");
 
            serializer.startTag("", "price");
            serializer.text(book.getPrice() + "");
            serializer.endTag("", "price");
 
            serializer.endTag("", "book");
        }
        serializer.endTag("", "books");
        serializer.endDocument();
 
        return writer.toString();
    }
}
```
```java
String xmlString = parser.serialize(booksList); // 序列化
                    FileOutputStream fos = openFileOutput("books.xml",
                            Context.MODE_PRIVATE);
                    fos.write(xmlString.getBytes("UTF-8"));
```