 利用Snackbar显示所有提示信息
```java
private void showMessage(String message) {
 Snackbar.make(getView(), message, Snackbar.LENGTH_LONG).show();
}
```



> @NonNull 用法

EditText左边显示图片
```xml
android:drawableLeft="@drawable/ic_action_person"
```