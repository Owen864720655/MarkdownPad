`mkdir` `FolderName`创建以`FolderName`为名字的文件夹或目录
`cd\`退到根目录
`cd ..`退到上一级目录
`pwd`显示当前的目录
`git init`把当前目录变为可以git管理的仓库
`git status`命令可以列出当前目录所有还没有被git管理的文件和被git管理且被修改但还未提交(git commit)的文件.。
`git commit-a` 命令的-a选项可只将所有被修改或者已删除的**且已经被git管理**的文档提交倒仓库中。
### Git提交文件到版本库
1. 使用`git add`将文件添加进去，实际就是将文件添加到暂存区(state)
2. 使用`git commit`把文件提交到仓库	`git commit -m '更改的注释'`



二、生成SSH密钥过程：
1.查看是否已经有了ssh密钥：cd ~/.ssh
如果没有密钥则不会有此文件夹，有则备份删除
2.生存密钥：
$ ssh-keygen -t rsa -C "1101320653@qq.com"
密码：261229


