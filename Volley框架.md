# Volley #
Volley的设计目标就是非常适合去进行数据量不大，但通信频繁的网络操作，而对于大数据量的网络操作，比如说下载文件等，Volley的表现就会非常糟糕。

----------
## Volley的操作步骤 ##
1. 获取RequestQueue对象    
```java
RequestQueue mQueue = Volley.newRequestQueue(context);
```
2. 建立一个请求
```java
//StringRequest的构造函数需要传入三个参数，第一个参数就是目标服务器的URL地址，第二个参数是服务器响应成功的回调，第三个参数是服务器响应失败的回调。
StringRequest stringRequest = new StringRequest("http://www.baidu.com",  
                        new Response.Listener<String>() {  
                            @Override  
                            public void onResponse(String response) {  
                                Log.d("TAG", response);  
                            }  
                        }, new Response.ErrorListener() {  
                            @Override  
                            public void onErrorResponse(VolleyError error) {  
                                Log.e("TAG", error.getMessage(), error);  
                            }  
                        }); 
```
3. 将请求添加进RequestQueue对象中。
```java
mQueue.add(stringRequest);
```
----------
## JsonRequest的用法##
```java
JsonObjectRequest jsonObjectRequest = new JsonObjectRequest("http://m.weather.com.cn/data/101010100.html", null,  
        new Response.Listener<JSONObject>() {  
            @Override  
            public void onResponse(JSONObject response) {  
                Log.d("TAG", response.toString());  
            }  
        }, new Response.ErrorListener() {  
            @Override  
            public void onErrorResponse(VolleyError error) {  
                Log.e("TAG", error.getMessage(), error);  
            }  
        });  

```
## ImageRequest的用法##
```java
ImageRequest imageRequest = new ImageRequest(  
        "http://developer.android.com/images/home/aw_dac.png",  
        new Response.Listener<Bitmap>() {  
            @Override  
            public void onResponse(Bitmap response) {  
                imageView.setImageBitmap(response);  
            }  
        }, 0, 0, Config.RGB_565, new Response.ErrorListener() {  
            @Override  
            public void onErrorResponse(VolleyError error) {  
                imageView.setImageResource(R.drawable.default_image);  
            }  
        });  
```
第三第四个参数分别用于指定允许图片最大的宽度和高度，如果指定的网络图片的宽度或高度大于这里的最大值，则会对图片进行压缩，指定成0的话就表示不管图片有多大，都不会进行压缩。第五个参数用于指定图片的颜色属性，Bitmap.Config下的几个常量都可以在这里使用，其中ARGB_8888可以展示最好的颜色属性，每个图片像素占据4个字节的大小，而RGB_565则表示每个图片像素占据2个字节大小。
## ImageLoader的用法##
ImageLoader不仅可以帮我们对图片进行缓存，还可以过滤掉重复的链接，避免重复发送请求。
[http://blog.csdn.net/guolin_blog/article/details/17482165](http://blog.csdn.net/guolin_blog/article/details/17482165 "Android Volley完全解析(二)，使用Volley加载网络图片")

----------

## 定制自己的Request##
```java
/**
 * A canned request for retrieving the response body at a given URL as a String.
 */
public class StringRequest extends Request<String> {
    private final Listener<String> mListener;

    /**
     * Creates a new request with the given method.
     *
     * @param method the request {@link Method} to use
     * @param url URL to fetch the string at
     * @param listener Listener to receive the String response
     * @param errorListener Error listener, or null to ignore errors
     */
    public StringRequest(int method, String url, Listener<String> listener,
            ErrorListener errorListener) {
        super(method, url, errorListener);
        mListener = listener;
    }

    /**
     * Creates a new GET request.
     *
     * @param url URL to fetch the string at
     * @param listener Listener to receive the String response
     * @param errorListener Error listener, or null to ignore errors
     */
    public StringRequest(String url, Listener<String> listener, ErrorListener errorListener) {
        this(Method.GET, url, listener, errorListener);
    }
//deliverResponse()方法调用mListener中的onResponse()方法，并将response内容传入即可，这样就可以将服务器响应的数据进行回调了。

    @Override
    protected void deliverResponse(String response) {
        mListener.onResponse(response);
    }
//对服务器响应的数据进行解析,其中数据是以字节的形式存放在NetworkResponse的data变量中的
    @Override
    protected Response<String> parseNetworkResponse(NetworkResponse response) {
        String parsed;
        try {
            parsed = new String(response.data, HttpHeaderParser.parseCharset(response.headers));
        } catch (UnsupportedEncodingException e) {
            parsed = new String(response.data);
        }
        return Response.success(parsed, HttpHeaderParser.parseCacheHeaders(response));
    }
}
```
Request类中的deliverResponse()和parseNetworkResponse()是两个抽象方法,必须在子类中实现