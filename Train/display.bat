@echo off  
call conda activate ml
cd D:\Unity\Project\training_system\Train  
tensorboard --logdir results --port 6006
pause
