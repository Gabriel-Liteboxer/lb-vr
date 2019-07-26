@ECHO OFF

ECHO Unistalling com.oculus.UnitySample

ECHO =============================

cd C:\Program Files\Unity\Hub\Editor\2019.1.5f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools

adb uninstall com.oculus.UnitySample

PAUSE