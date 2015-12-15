
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

DELETE FROM `action`;
ALTER TABLE `action`
  DROP COLUMN `Name`,
  DROP COLUMN `Description`,
  DROP COLUMN `DamagePhysicalAttribute`,
  ADD COLUMN `TextKey` varchar(32) NOT NULL AFTER `ID`,
  CHANGE COLUMN `AttributeIntercept` `AttributeIntercept` float NOT NULL,
  CHANGE COLUMN `AttributeSlope` `AttributeSlope` float NOT NULL,
  CHANGE COLUMN `Intercept1` `Intercept1` float NOT NULL,
  CHANGE COLUMN `Slope1` `Slope1` float NOT NULL,
  CHANGE COLUMN `Intercept2` `Intercept2` float NOT NULL,
  CHANGE COLUMN `Slope2` `Slope2` float NOT NULL,
  CHANGE COLUMN `Intercept3` `Intercept3` float NOT NULL,
  CHANGE COLUMN `Slope3` `Slope3` float NOT NULL,
  ADD UNIQUE KEY `i1` (`TextKey`);

ALTER TABLE `attribute`
  DROP COLUMN `Name`,
  DROP KEY `ID_UNIQUE`;

DROP TABLE `battle_drop`;

ALTER TABLE `character_effect`
  DROP KEY `ID_UNIQUE`;

DELETE FROM `characters`;
ALTER TABLE `characters`
  DROP COLUMN `Name`,
  DROP COLUMN `Description`,
  ADD COLUMN `TextKey` varchar(32) NOT NULL AFTER `ID`,
  ADD UNIQUE KEY `i1` (`TextKey`);
ALTER TABLE `characters` RENAME `character`;

DROP TABLE `cooperate_character`;
CREATE TABLE `cooperation` (
  `ID` int unsigned NOT NULL,
  `TextKey` varchar(32) NOT NULL,
  `CharacterID1` int unsigned NOT NULL,
  `CharacterID2` int unsigned NOT NULL,
  `CharacterID3` int unsigned NOT NULL,
  `CharacterID4` int unsigned NOT NULL,
  `CharacterID5` int unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `i1` (`TextKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE `cooperate_level`;
CREATE TABLE `cooperation_parameter` (
  `CooperationID` int unsigned NOT NULL,
  `Level` tinyint unsigned NOT NULL,
  `Strength` float unsigned NOT NULL,
  `Intellect` float unsigned NOT NULL,
  `Agility` float unsigned NOT NULL,
  `PhysicalAttack` int unsigned NOT NULL,
  `MagicPower` int unsigned NOT NULL,
  `Armor` int unsigned NOT NULL,
  `MagicRegistance` int unsigned NOT NULL,
  `Accuracy` tinyint unsigned NOT NULL,
  `Dodge` tinyint unsigned NOT NULL,
  `ActionFrequency` float unsigned NOT NULL,
  PRIMARY KEY (`CooperationID`,`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

ALTER TABLE `cpu_battle_story`
  CHANGE COLUMN `DramaType` `DramaType` tinyint unsigned NOT NULL;

DELETE FROM `cpu_battle_story_message`;
ALTER TABLE `cpu_battle_story_message`
  DROP COLUMN `Content`,
  ADD COLUMN `TextKey` varchar(32) NOT NULL AFTER `No`,
  ADD UNIQUE KEY `i1` (`TextKey`);

DELETE FROM `daily_quest`;
ALTER TABLE `daily_quest`
  DROP COLUMN `Name`,
  DROP COLUMN `Description`,
  ADD COLUMN `TextKey` varchar(32) NOT NULL AFTER `ID`,
  ADD UNIQUE KEY `i2` (`TextKey`);

ALTER TABLE `gacha_content`
  CHANGE COLUMN `ContentID` `ID` int unsigned NOT NULL;

DELETE FROM `quest`;
ALTER TABLE `quest`
  DROP COLUMN `Name`,
  DROP COLUMN `Description`,
  ADD COLUMN `TextKey` varchar(32) NOT NULL AFTER `ID`,
  ADD UNIQUE KEY `i2` (`TextKey`);

DROP TABLE `shadow_character`;
CREATE TABLE `shadow_character` (
  `ID` int unsigned NOT NULL,
  `CharacterID` int unsigned NOT NULL,
  `Level` smallint unsigned NOT NULL,
  `PromotionLevel` tinyint unsigned NOT NULL,
  `EvolutionLevel` tinyint unsigned NOT NULL,
  `ActionLevel1` smallint unsigned NOT NULL,
  `ActionLevel2` smallint unsigned NOT NULL,
  `ActionLevel3` smallint unsigned NOT NULL,
  `ActionLevel4` smallint unsigned NOT NULL,
  `ActionLevel5` smallint unsigned NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DELETE FROM `shop`;
ALTER TABLE `shop`
  DROP COLUMN `Name`,
  DROP COLUMN `Description`,
  ADD COLUMN `TextKey` varchar(32) NOT NULL AFTER `ID`,
  ADD UNIQUE KEY `i1` (`TextKey`);

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back


DROP TABLE `action`;
CREATE TABLE `action` (
  `ID` int(10) unsigned NOT NULL,
  `ClassID` int(10) unsigned NOT NULL,
  `TriggerID` tinyint(3) unsigned NOT NULL,
  `ExecutableProbability` float unsigned NOT NULL,
  `ExecutableNum` tinyint(3) unsigned NOT NULL,
  `Name` text NOT NULL,
  `Description` text NOT NULL,
  `TargetSortType` tinyint(3) unsigned NOT NULL,
  `MaxTargetNum` tinyint(3) unsigned NOT NULL,
  `DamageType` tinyint(3) unsigned NOT NULL,
  `DamagePhysicalAttribute` tinyint(3) unsigned NOT NULL,
  `DamageMagicalAttribute` tinyint(3) unsigned NOT NULL,
  `TargetStateID` tinyint(3) unsigned NOT NULL,
  `AttributeID` int(10) unsigned NOT NULL,
  `AttributeIntercept` int(11) NOT NULL,
  `AttributeSlope` int(11) NOT NULL,
  `Arg1` int(11) NOT NULL,
  `Arg2` int(11) NOT NULL,
  `Arg3` int(11) NOT NULL,
  `Arg4` int(11) NOT NULL,
  `Arg5` int(11) NOT NULL,
  `Arg6` int(11) NOT NULL,
  `Arg7` int(11) NOT NULL,
  `Arg8` int(11) NOT NULL,
  `Arg9` int(11) NOT NULL,
  `Arg10` int(11) NOT NULL,
  `Intercept1` int(11) NOT NULL,
  `Slope1` int(11) NOT NULL,
  `Intercept2` int(11) NOT NULL,
  `Slope2` int(11) NOT NULL,
  `Intercept3` int(11) NOT NULL,
  `Slope3` int(11) NOT NULL,
  `HitCharacterEffectID` int(10) unsigned NOT NULL,
  `SoundID` int(10) unsigned NOT NULL,
  `ExecutionSoundID` int(10) unsigned NOT NULL,
  `HitSoundID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

ALTER TABLE `attribute`
  ADD COLUMN `Name` text NOT NULL AFTER `ClassID`,
  ADD KEY `ID_UNIQUE` (`ID`);

CREATE TABLE `battle_drop` (
  `BattleID` int(10) unsigned NOT NULL,
  `ItemID` int(10) unsigned NOT NULL,
  `Ratio` int(10) unsigned NOT NULL,
  PRIMARY KEY (`BattleID`,`ItemID`),
  UNIQUE KEY `BattleID_UNIQUE` (`BattleID`),
  UNIQUE KEY `ItemID_UNIQUE` (`ItemID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

ALTER TABLE `character_effect`
  ADD UNIQUE KEY `ID_UNIQUE` (`ID`);

ALTER TABLE `character` RENAME `characters`;
ALTER TABLE `characters`
  ADD COLUMN `Name` text NOT NULL AFTER `TagFlags`,
  ADD COLUMN `Description` text NOT NULL AFTER `Name`,
  DROP COLUMN `TextKey`;

DROP TABLE `cooperation`;
CREATE TABLE `cooperate_character` (
  `CooperateID` int(10) unsigned NOT NULL,
  `Name` varchar(45) NOT NULL,
  `CharacterID1` int(10) unsigned NOT NULL,
  `CharacterID2` int(10) unsigned NOT NULL,
  `CharacterID3` int(10) unsigned NOT NULL DEFAULT '0',
  `CharacterID4` int(10) unsigned NOT NULL DEFAULT '0',
  `CharacterID5` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`CooperateID`),
  UNIQUE KEY `CharacterID2_UNIQUE` (`CharacterID2`),
  UNIQUE KEY `CharacterID1_UNIQUE` (`CharacterID1`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE `cooperation_parameter`;
CREATE TABLE `cooperate_level` (
  `CooperateID` int(10) unsigned NOT NULL,
  `Level` int(10) unsigned NOT NULL,
  `PhysicalAttack` float unsigned NOT NULL,
  `MagicalAttack` float unsigned NOT NULL,
  `PhysicalDefense` float unsigned NOT NULL,
  `MagicDefense` float unsigned NOT NULL,
  `Dodge` tinyint(3) unsigned NOT NULL,
  `Accuracy` tinyint(3) unsigned NOT NULL,
  `Strength` float unsigned NOT NULL,
  `Intellect` float unsigned NOT NULL,
  `Technique` float unsigned NOT NULL,
  `Speed` float unsigned NOT NULL,
  PRIMARY KEY (`CooperateID`,`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

ALTER TABLE `cpu_battle_story`
  CHANGE COLUMN `DramaType` `DramaType` tinyint NOT NULL;

ALTER TABLE `cpu_battle_story_message`
  ADD COLUMN `Content` text NOT NULL AFTER `EnemyCharacterID`,
  DROP COLUMN `TextKey`;

ALTER TABLE `daily_quest`
  ADD COLUMN `Name` text NOT NULL AFTER `Threshold`,
  ADD COLUMN `Description` text NOT NULL AFTER `Name`,
  DROP COLUMN `TextKey`;

ALTER TABLE `gacha_content`
  CHANGE COLUMN `ID` `ContentID` int unsigned NOT NULL;

ALTER TABLE `quest`
  ADD COLUMN `Name` text NOT NULL AFTER `Threshold`,
  ADD COLUMN `Description` text NOT NULL AFTER `Name`,
  DROP COLUMN `TextKey`;

DROP TABLE `shadow_character`;
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

ALTER TABLE `shop`
  ADD COLUMN `Name` text NOT NULL AFTER `ResidentVipLevel`,
  ADD COLUMN `Description` text NOT NULL AFTER `Name`,
  DROP COLUMN `TextKey`;


