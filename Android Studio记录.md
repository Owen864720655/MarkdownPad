#Android Studio记录#
引进RxJava包
在`buildTypes`下面添加
```java
repositories {
            jcenter {
                url "http://jcenter.bintray.com/"
            }
        }
```
webservice支持引入jar
在repositories下面添加

    maven { url 'http://ksoap2-android.googlecode.com/svn/m2-repo' }
在`dependencies`下面添加

    compile 'com.google.code.ksoap2-android:ksoap2-android:3.3.0'

有的需要翻墙才能加载的包 解决办法
```java
allprojects {  
    repositories {  
        mavenCentral()  
    }  
}  
```