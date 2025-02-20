@echo off  
call conda activate ml  
cd D:\Unity\Project\training_system\Train  
mlagents-learn test.yaml --run-id test --torch-device cuda:0  
pause