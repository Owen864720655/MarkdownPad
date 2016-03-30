回来再整理
所需要的jar包
```java
    compile 'com.squareup.retrofit2:retrofit:2.0.0'
    compile 'com.google.code.gson:gson:2.3'
    compile 'com.squareup.retrofit2:converter-gson:2.0.0-beta4'
```
步骤：
1. 新建service 接口
```java
public interface gitapi {
    @GET("users/{user}/repos")
    Call<gitmodel> listRepos(@Path("user") String user);
    @GET("http://www.baidu.com/")
    Call<ResponseBody> get();
//电话信息
    @GET("mobilenumber")
    Call<Example> getPhone(@Query("phone") String phoneNumber,@Header("apikey")String apikey);
}
```

2. 新建Json解析类
 ```java
public class Example {
    private Integer errNum;
    private String retMsg;
    private RetData retData;

    public Integer getErrNum() {
        return errNum;
    }

    public void setErrNum(Integer errNum) {
        this.errNum = errNum;
    }

    public String getRetMsg() {
        return retMsg;
    }

    public void setRetMsg(String retMsg) {
        this.retMsg = retMsg;
    }

    public RetData getRetData() {
        return retData;
    }

    public void setRetData(RetData retData) {
        this.retData = retData;
    }
}
```
```java
package com.com.rxjava.test;

/**
 * Created by yth on 2016/3/29.
 */
public class RetData {
    private String phone;
    private String prefix;
    private String supplier;
    private String province;
    private String city;
    private String suit;

    public String getPhone() {
        return phone;
    }

    public void setPhone(String phone) {
        this.phone = phone;
    }

    public String getPrefix() {
        return prefix;
    }

    public void setPrefix(String prefix) {
        this.prefix = prefix;
    }

    public String getSupplier() {
        return supplier;
    }

    public void setSupplier(String supplier) {
        this.supplier = supplier;
    }

    public String getProvince() {
        return province;
    }

    public void setProvince(String province) {
        this.province = province;
    }

    public String getCity() {
        return city;
    }

    public void setCity(String city) {
        this.city = city;
    }

    public String getSuit() {
        return suit;
    }

    public void setSuit(String suit) {
        this.suit = suit;
    }
}

```
3. 执行调用
```java
                Retrofit retrofit = new Retrofit.Builder()
                        .baseUrl("http://apis.baidu.com/apistore/mobilenumber/")
                        .addConverterFactory(GsonConverterFactory.create())
                        .build();
                gitapi git = retrofit.create(gitapi.class);
                Call<Example> call = git.getPhone("18451906106", "86c64229efb06fd4715a5abd8a05db04");
                call.enqueue(new Callback<Example>() {
                    @Override
                    public void onResponse(Call<Example> call, Response<Example> response) {
                        Example em = response.body();
                        Log.i("xxxxx", em.getRetData().getPhone());
                    }

                    @Override
                    public void onFailure(Call<Example> call, Throwable t) {
                        Log.i("error", t.toString());
                    }
                });
```
##url细节##
![](http://i.imgur.com/1S2oq8b.png)
一般就用第二个
对于 Retrofit 2.0中新的URL定义方式
- Base URL: 总是以 /结尾
- @Url: 不要以 / 开头

##关于请求参数的设定##

1.url路径上有参数
```java
   @GET("group/{id}/users")
   Call<List<User>> groupList(@Path("id") int groupId);
```
@Get里边是一个相对路径，相对路径上有一个变量，那这个时候变量的就用{id}括起来
然后在请求参数上添加@Path("id") 就可以在请求中赋值
例如
```java
```
