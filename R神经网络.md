R语言中已经有许多用于神经网络的package。例如nnet、AMORE以及neuralnet，nnet提供了最常见的前馈反向传播神经网络算法。AMORE包则更进一步提供了更为丰富的控制参数，并可以增加多个隐藏层。neuralnet包的改进在于提供了弹性反向传播算法和更多的激活函数形式。但以上各包均围绕着BP网络，并未涉及到神经网络中的其它拓扑结构和网络模型。而新出炉的RSNNS包则在这方面有了极大的扩充。 

Stuttgart Neural Network Simulator（SNNS）是德国斯图加特大学开发的优秀神经网络仿真软件，为国外的神经网络研究者所广泛采用。其手册内容极为丰富，同时支持友好的 Linux 平台。而RSNNS则是连接R和SNNS的工具，在R中即可直接调用SNNS的函数命令。 

#载入程序和数据 
library(RSNNS) 
data(iris)
#将数据顺序打乱 
iris = iris[sample(1:nrow(iris),length(1:nrow(iris))),1:ncol(iris)]
#定义网络输入 
irisValues= iris[,1:4]
#定义网络输出，并将数据进行格式转换 
irisTargets = decodeClassLabels(iris[,5])
#从中划分出训练样本和检验样本 
iris = splitForTrainingAndTest(irisValues, irisTargets, ratio=0.15)
#数据标准化 
iris = normTrainingAndTestSet(iris)
#利用mlp命令执行前馈反向传播神经网络算法 
model = mlp(iris$inputsTrain, iris$targetsTrain, size=5, learnFunc="Quickprop", learnFuncParams=c(0.1, 2.0, 0.0001, 0.1),maxit=100, inputsTest=iris$inputsTest, targetsTest=iris$targetsTest)
#利用上面建立的模型进行预测 
predictions = predict(model,iris$inputsTest)
#生成混淆矩阵，观察预测精度 
confusionMatrix(iris$targetsTest,predictions)
结果如下： 

       predictions 
targets  1  2  3 
      1  7  0  0 
      2  0  5  0 
      3  0  1 10 

本例中mlp意指多层感知器，RSNNS包中其它重要的网络形式还包括： dlvq（动态学习向量化网络）, rbf（径向基函数网络）, elman（elman神经网络）, jordan（jordan神经网络）, som（自组织映射神经网络）, art1（适应性共振神经网络）等等 


R语言BP神经网络建模newff,train,sim函数详解http://blog.sina.com.cn/s/blog_c40025360102wyzx.html