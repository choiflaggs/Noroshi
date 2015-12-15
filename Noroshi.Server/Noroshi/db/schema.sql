CREATE TABLE `action` (
  `ID` int(10) unsigned NOT NULL,
  `ClassID` int(10) unsigned NOT NULL,
  `TriggerID` int(10) unsigned NOT NULL,
  `Name` varchar(64) NOT NULL,
  `Description` varchar(280) NOT NULL,
  `Rank` int(10) unsigned NOT NULL,
  `Arg1` int(10) unsigned NOT NULL,
  `Arg2` int(10) unsigned NOT NULL,
  `Arg3` int(10) unsigned NOT NULL,
  `Arg4` int(10) unsigned NOT NULL,
  `Arg5` int(10) unsigned NOT NULL,
  `Arg6` int(10) unsigned NOT NULL,
  `Arg7` int(10) unsigned NOT NULL,
  `Arg8` int(10) unsigned NOT NULL,
  `Arg9` int(10) unsigned NOT NULL,
  `Arg10` int(10) unsigned NOT NULL,
  `Intercept1` int(10) unsigned NOT NULL,
  `Slope1` int(10) unsigned NOT NULL,
  `Intercept2` int(10) unsigned NOT NULL,
  `Slope2` int(10) unsigned NOT NULL,
  `Intercept3` int(10) unsigned NOT NULL,
  `Slope3` int(10) unsigned NOT NULL,
  `HitCharacterEffectID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `attribute` (
  `ID` int(10) unsigned NOT NULL,
  `ClassID` int(10) unsigned NOT NULL,
  `Name` varchar(64) NOT NULL,
  `GroupID` int(10) unsigned NOT NULL,
  `Lifetime` int(10) unsigned NOT NULL,
  `EffectID` int(10) unsigned NOT NULL,
  `Arg1` int(10) unsigned NOT NULL,
  `Arg2` int(10) unsigned NOT NULL,
  `Arg3` int(10) unsigned NOT NULL,
  `Arg4` int(10) unsigned NOT NULL,
  `Arg5` int(10) unsigned NOT NULL,
  `Arg6` int(10) unsigned NOT NULL,
  `Arg7` int(10) unsigned NOT NULL,
  `Arg8` int(10) unsigned NOT NULL,
  `Arg9` int(10) unsigned NOT NULL,
  `Arg10` int(10) unsigned NOT NULL,
  `ReceiveDamageCharacterEffectID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `battle` (
  `ID` int(10) unsigned NOT NULL,
  `Stamina` int(10) unsigned NOT NULL,
  `Exp` int(10) unsigned NOT NULL,
  `Money` int(10) unsigned NOT NULL,
  `MinDropItemNum` int(10) unsigned NOT NULL,
  `MaxDropItemNum` int(10) unsigned NOT NULL,
  `BossEnemyCharacterID` int(10) unsigned NOT NULL,
  `BossStoneDropRatio` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `battle_drop` (
  `BattleID` int(10) unsigned NOT NULL,
  `ItemID` int(10) unsigned NOT NULL,
  `Ratio` int(10) unsigned NOT NULL,
  PRIMARY KEY (`BattleID`,`ItemID`),
  UNIQUE KEY `BattleID_UNIQUE` (`BattleID`),
  UNIQUE KEY `ItemID_UNIQUE` (`ItemID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `battle_wave` (
  `ID` int(10) unsigned NOT NULL,
  `BattleID` int(11) NOT NULL,
  `EnemyCharacterID1` int(10) unsigned NOT NULL,
  `EnemyCharacterID2` int(10) unsigned NOT NULL,
  `EnemyCharacterID3` int(10) unsigned NOT NULL,
  `EnemyCharacterID4` int(10) unsigned NOT NULL,
  `EnemyCharacterID5` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `chapter` (
  `ID` int(10) unsigned NOT NULL,
  `Name` varchar(64) NOT NULL,
  `Level` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `character_effect` (
  `ID` int(10) unsigned NOT NULL,
  `PrefabID` int(10) unsigned NOT NULL,
  `AnimationName` varchar(64) NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `character_exp` (
  `Level` int(10) unsigned NOT NULL,
  `Exp` int(10) unsigned NOT NULL,
  PRIMARY KEY (`Level`),
  UNIQUE KEY `Level_UNIQUE` (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `character_gear` (
  `CharacterID` int(10) unsigned NOT NULL,
  `Level` int(10) unsigned NOT NULL,
  `Gear1` int(10) unsigned NOT NULL,
  `Gear2` int(10) unsigned NOT NULL,
  `Gear3` int(10) unsigned NOT NULL,
  `Gear4` int(10) unsigned NOT NULL,
  `Gear5` int(10) unsigned NOT NULL,
  `Gear6` int(10) unsigned NOT NULL,
  PRIMARY KEY (`CharacterID`,`Level`),
  UNIQUE KEY `CharacterID_UNIQUE` (`CharacterID`),
  UNIQUE KEY `character_gearcol_UNIQUE` (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `characters` (
  `ID` int(10) unsigned NOT NULL,
  `Name` varchar(64) NOT NULL,
  `InitialEvolutionLevel` int(10) unsigned NOT NULL,
  `Position` int(10) unsigned NOT NULL,
  `OrderPriority` int(10) unsigned NOT NULL,
  `Type` tinyint(1) unsigned NOT NULL,
  `StrengthGrowth` float unsigned NOT NULL,
  `IntellectGrowth` float unsigned NOT NULL,
  `AgilityGrowth` float unsigned NOT NULL,
  `Strength` int(10) unsigned NOT NULL,
  `Intellect` int(10) unsigned NOT NULL,
  `Agility` int(10) unsigned NOT NULL,
  `MagicCrit` float unsigned NOT NULL,
  `HPRegen` float unsigned NOT NULL,
  `EnergyRegen` float unsigned NOT NULL,
  `Dodge` float unsigned NOT NULL,
  `ArmorPenetration` float unsigned NOT NULL,
  `LifeStealRating` float unsigned NOT NULL,
  `ImproveHealings` float unsigned NOT NULL,
  `IgnoreMagicResistance` float unsigned NOT NULL,
  `ActionID0` int(10) unsigned NOT NULL,
  `ActionID1` int(10) unsigned NOT NULL,
  `ActionID2` int(10) unsigned NOT NULL,
  `ActionID3` int(10) unsigned NOT NULL,
  `ActionID4` int(10) unsigned NOT NULL,
  `ActionID5` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `enemy_character` (
  `ID` int(10) unsigned NOT NULL,
  `CharacterID` int(10) unsigned NOT NULL,
  `Level` int(10) unsigned NOT NULL,
  `PromotionLevel` int(10) unsigned NOT NULL,
  `EvolutionLevel` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `gear` (
  `ItemID` int(10) unsigned NOT NULL,
  `Level` int(10) unsigned NOT NULL,
  `HP` int(10) unsigned NOT NULL,
  `StrengthGrowth` float unsigned NOT NULL,
  `IntellectGrowth` float unsigned NOT NULL,
  `AgilityGrowth` float unsigned NOT NULL,
  `Strength` int(10) unsigned NOT NULL,
  `Intellect` int(10) unsigned NOT NULL,
  `Agility` int(10) unsigned NOT NULL,
  `MagicCrit` float unsigned NOT NULL,
  `HPRegen` float unsigned NOT NULL,
  `EnergyRegen` float unsigned NOT NULL,
  `Dodge` float unsigned NOT NULL,
  `ArmorPenetration` float unsigned NOT NULL,
  `LifeStealRating` float unsigned NOT NULL,
  `ImproveHealings` float unsigned NOT NULL,
  `IgnoreMagicResistance` float unsigned NOT NULL,
  `PhysicalAttack` float unsigned NOT NULL,
  `MagicPower` float unsigned NOT NULL,
  `Armor` float unsigned NOT NULL,
  `MagicResistance` float unsigned NOT NULL,
  `PhysicalCrit` float unsigned NOT NULL,
  `Accuracy` float unsigned NOT NULL,
  PRIMARY KEY (`ItemID`),
  UNIQUE KEY `ItemID_UNIQUE` (`ItemID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `gear_recipe` (
  `ID` int(10) unsigned NOT NULL,
  `MaterialItemID` int(10) unsigned NOT NULL,
  `CraftItemID` int(10) unsigned NOT NULL,
  `Count` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `item` (
  `ID` int(10) unsigned NOT NULL,
  `Name` varchar(64) NOT NULL,
  `FlavorText` varchar(208) NOT NULL,
  `Price` int(11) NOT NULL,
  `Type` int(11) NOT NULL,
  `Reality` int(11) NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player` (
  `iD` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `UDID` varchar(128) NOT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`iD`),
  UNIQUE KEY `iD_UNIQUE` (`iD`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=latin1;
CREATE TABLE `player_battle_session` (
  `ID` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `PlayerID` int(10) unsigned NOT NULL,
  `SessionID` varchar(64) NOT NULL,
  `PlayerCharacterID1` int(10) unsigned NOT NULL,
  `PlayerCharacterID2` int(10) unsigned NOT NULL,
  `PlayerCharacterID3` int(10) unsigned NOT NULL,
  `PlayerCharacterID4` int(10) unsigned NOT NULL,
  `PlayerCharacterID5` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `i1` (`PlayerID`),
  UNIQUE KEY `i2` (`SessionID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player_character` (
  `PlayerCharacterID` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `PlayerID` int(10) unsigned NOT NULL,
  `CharacterID` int(10) unsigned NOT NULL,
  `Level` int(10) unsigned NOT NULL DEFAULT '1',
  `Exp` int(10) unsigned NOT NULL DEFAULT '0',
  `PromotionLevel` int(10) unsigned NOT NULL DEFAULT '1',
  `EvolutionLevel` int(10) unsigned NOT NULL DEFAULT '1',
  `ActionLevel1` int(10) unsigned NOT NULL DEFAULT '1',
  `ActionLevel2` int(10) unsigned NOT NULL DEFAULT '1',
  `ActionLevel3` int(10) unsigned NOT NULL DEFAULT '1',
  `ActionLevel4` int(10) unsigned NOT NULL DEFAULT '1',
  `ActionLevel5` int(10) unsigned NOT NULL DEFAULT '1',
  `ActionLevel6` int(10) unsigned NOT NULL DEFAULT '1',
  `Gear1` int(10) unsigned NOT NULL DEFAULT '0',
  `Gear2` int(10) unsigned NOT NULL DEFAULT '0',
  `Gear3` int(10) unsigned NOT NULL DEFAULT '0',
  `Gear4` int(10) unsigned NOT NULL DEFAULT '0',
  `Gear5` int(10) unsigned NOT NULL DEFAULT '0',
  `Gear6` int(10) unsigned NOT NULL DEFAULT '0',
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`PlayerCharacterID`),
  UNIQUE KEY `PlayerCharacterID_UNIQUE` (`PlayerCharacterID`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=latin1;
CREATE TABLE `player_exp` (
  `Level` int(10) unsigned NOT NULL,
  `Exp` int(10) unsigned NOT NULL,
  PRIMARY KEY (`Level`),
  UNIQUE KEY `level_UNIQUE` (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player_status` (
  `PlayerID` int(10) unsigned NOT NULL,
  `Language` int(10) unsigned NOT NULL,
  `Name` varchar(64) DEFAULT NULL,
  `Exp` int(10) unsigned DEFAULT '0',
  `VipExp` int(10) unsigned DEFAULT '0',
  `GuildID` int(10) unsigned DEFAULT '0',
  `StaminaTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `Gold` int(10) unsigned DEFAULT '0',
  `FreeGem` int(10) unsigned DEFAULT '0',
  `ChargeGem` int(10) unsigned DEFAULT '0',
  `AvaterCharacterID` int(10) unsigned DEFAULT '11',
  `CreatedAt` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`PlayerID`),
  UNIQUE KEY `PlayerID_UNIQUE` (`PlayerID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
CREATE TABLE `quest` (
  `ID` int(10) unsigned NOT NULL,
  `ChapterID` int(10) unsigned NOT NULL,
  `Type` int(10) unsigned NOT NULL,
  `BattleID` int(10) unsigned NOT NULL,
  `Name` varchar(64) NOT NULL,
  `Description` varchar(280) NOT NULL,
  `RemainChance` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `shadow_character` (
  `ID` int(10) unsigned NOT NULL,
  `CharacterID` int(10) unsigned NOT NULL,
  `Level` int(10) unsigned NOT NULL,
  `PromotionLevel` int(10) unsigned NOT NULL,
  `EvolutionLevel` int(10) unsigned NOT NULL,
  `ActionLevel1` int(10) unsigned NOT NULL,
  `ActionLevel2` int(10) unsigned NOT NULL,
  `ActionLevel3` int(10) unsigned NOT NULL,
  `ActionLevel4` int(10) unsigned NOT NULL,
  `ActionLevel5` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
