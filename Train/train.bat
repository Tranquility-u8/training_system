  
@echo off  
call conda activate ml  
cd D:\Unity\Project\training_system\Train  
mlagents-learn Test1.yaml --run-id Test1_20250220231838 --torch-device cuda:0  
pause  
