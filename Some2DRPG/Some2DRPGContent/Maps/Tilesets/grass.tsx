<?xml version="1.0" encoding="UTF-8"?>
<tileset name="grass" tilewidth="32" tileheight="32">
 <properties>
  <property name="Content" value="LPC/Terrain/grass"/>
 </properties>
 <image source="../../LPC/Terrain/grass.png" width="96" height="192"/>
 <terraintypes>
  <terrain name="Grass" tile="-1"/>
 </terraintypes>
 <tile id="1" terrain="0,0,0,"/>
 <tile id="2" terrain="0,0,,0"/>
 <tile id="4" terrain="0,,0,0"/>
 <tile id="5" terrain=",0,0,0"/>
 <tile id="6" terrain=",,,0"/>
 <tile id="7" terrain=",,0,0"/>
 <tile id="8" terrain=",,0,"/>
 <tile id="9" terrain=",0,,0"/>
 <tile id="10" terrain="0,0,0,0">
  <properties>
   <property name="MoveSpeed" value="0.6"/>
  </properties>
 </tile>
 <tile id="11" terrain="0,,0,"/>
 <tile id="12" terrain=",0,,"/>
 <tile id="13" terrain="0,0,,"/>
 <tile id="14" terrain="0,,,"/>
</tileset>
