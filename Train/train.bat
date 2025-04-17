  
@echo off  
call conda activate ml  
cd D:\Unity\Project\training_system\Train  
mlagents-learn MjReacher.yaml --run-id MjReacher_20250417220411 --torch-device cuda:0  
pause  
