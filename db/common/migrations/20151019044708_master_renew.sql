
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
ALTER TABLE `chapter` 
DROP COLUMN `Description` ,
CHANGE COLUMN `Name` `TextKey` VARCHAR(32) NOT NULL ,
CHANGE COLUMN `Level` `PlayerLevel` SMALLINT UNSIGNED NOT NULL ,
ADD COLUMN `No` SMALLINT UNSIGNED NOT NULL AFTER `ID`,
DROP INDEX `ID_UNIQUE`, RENAME TO `story_chapter`;

ALTER TABLE `character` 
ADD COLUMN `EvolutionType` SMALLINT UNSIGNED NOT NULL AFTER `Type`;

DROP TABLE `character_evolution`;

CREATE TABLE `character_evolution_type` (
  `Type` SMALLINT UNSIGNED NOT NULL,
  `EvolutionLevel` TINYINT UNSIGNED NOT NULL,
  `Soul` SMALLINT UNSIGNED NOT NULL,
  `NecessaryGold` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`Type`, `EvolutionLevel`));

ALTER TABLE `character_gear` 
CHANGE COLUMN `Level` `PromotionLevel` TINYINT UNSIGNED NOT NULL ,
CHANGE COLUMN `Gear1` `GearID1` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `Gear2` `GearID2` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `Gear3` `GearID3` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `Gear4` `GearID4` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `Gear5` `GearID5` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `Gear6` `GearID6` INT(10) UNSIGNED NOT NULL ;

DROP TABLE `defensive_war`;

ALTER TABLE `drug` 
CHANGE COLUMN `Exp` `CharacterExp` INT(10) UNSIGNED NOT NULL ;

ALTER TABLE `episode` 
DROP COLUMN `CharacterID3`,
DROP COLUMN `CharacterID2`,
DROP COLUMN `CharacterID1`,
DROP COLUMN `AfterEndEpisode`,
CHANGE COLUMN `EpisodeID` `ID` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `Name` `TextKey` VARCHAR(32) NOT NULL ,
ADD COLUMN `No` SMALLINT UNSIGNED NOT NULL AFTER `ID`,
ADD UNIQUE INDEX `No_ChapterID_UNIQUE` (`No`, `ChapterID`), RENAME TO `story_episode` ;

ALTER TABLE `gear` 
DROP COLUMN `Gem`,
DROP COLUMN `Gold5`,
DROP COLUMN `Gold4`,
DROP COLUMN `Gold3`,
DROP COLUMN `Gold2`,
DROP COLUMN `Gold1`,
DROP COLUMN `Exp5`,
DROP COLUMN `Exp4`,
DROP COLUMN `Exp3`,
DROP COLUMN `Exp2`,
DROP COLUMN `Exp1`,
DROP COLUMN `GrowthRate5`,
DROP COLUMN `GrowthRate4`,
DROP COLUMN `GrowthRate3`,
DROP COLUMN `GrowthRate2`,
DROP COLUMN `GrowthRate1`,
DROP COLUMN `EnchantExp`,
CHANGE COLUMN `PhysicalAttack` `PhysicalAttack` INT UNSIGNED NOT NULL ,
CHANGE COLUMN `MagicPower` `MagicPower` INT UNSIGNED NOT NULL ,
CHANGE COLUMN `Armor` `Armor` INT UNSIGNED NOT NULL ,
CHANGE COLUMN `MagicResistance` `MagicResistance` INT UNSIGNED NOT NULL ,
CHANGE COLUMN `PhysicalCrit` `PhysicalCrit` INT UNSIGNED NOT NULL ,
CHANGE COLUMN `Accuracy` `Accuracy` TINYINT UNSIGNED NOT NULL , 
CHANGE COLUMN `Strength` `Strength` FLOAT UNSIGNED NOT NULL AFTER `HP`,
CHANGE COLUMN `Intellect` `Intellect` FLOAT UNSIGNED NOT NULL AFTER `Strength`,
CHANGE COLUMN `Agility` `Agility` FLOAT UNSIGNED NOT NULL AFTER `Intellect`,
CHANGE COLUMN `MagicCrit` `MagicCrit` INT UNSIGNED NOT NULL AFTER `PhysicalCrit`,
CHANGE COLUMN `ArmorPenetration` `ArmorPenetration` INT UNSIGNED NOT NULL AFTER `MagicCrit`,
CHANGE COLUMN `IgnoreMagicResistance` `IgnoreMagicResistance` INT UNSIGNED NOT NULL AFTER `ArmorPenetration`,
CHANGE COLUMN `Dodge` `Dodge` FLOAT UNSIGNED NOT NULL AFTER `Accuracy`,
CHANGE COLUMN `HPRegen` `HPRegen` FLOAT UNSIGNED NOT NULL AFTER `Dodge`,
CHANGE COLUMN `EnergyRegen` `EnergyRegen` FLOAT UNSIGNED NOT NULL AFTER `HPRegen`,
CHANGE COLUMN `ImproveHealings` `ImproveHealings` FLOAT UNSIGNED NOT NULL AFTER `EnergyRegen`,
CHANGE COLUMN `LifeStealRating` `LifeStealRating` FLOAT UNSIGNED NOT NULL AFTER `ImproveHealings`;

CREATE TABLE `gear_enchant_level` (
  `GearID` INT UNSIGNED NOT NULL,
  `EnchantLevel` TINYINT UNSIGNED NOT NULL,
  `GrowthRate` FLOAT UNSIGNED NOT NULL,
  `Exp` INT UNSIGNED NOT NULL,
  `Gold` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`GearID`, `EnchantLevel`));

CREATE TABLE `gear_enchant_exp` (
  `GearID` INT UNSIGNED NOT NULL,
  `EnchantExp` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`GearID`));

ALTER TABLE `gear_enchant_material` 
CHANGE COLUMN `Exp` `EnchantExp` INT(10) UNSIGNED NOT NULL ;

ALTER TABLE `gear_piece` 
CHANGE COLUMN `Exp` `EnchantExp` INT(10) UNSIGNED NOT NULL ;

ALTER TABLE `gear_recipe` 
CHANGE COLUMN `MaterialType` `MaterialType` TINYINT UNSIGNED NOT NULL, 
DROP INDEX `ID_UNIQUE` ;

ALTER TABLE `item` 
DROP COLUMN `FlavorText`,
CHANGE COLUMN `Name` `TextKey` VARCHAR(32) NOT NULL ,
DROP INDEX `ID_UNIQUE` ;

DROP TABLE `raid_ticket`;

ALTER TABLE `skill_level_price` 
CHANGE COLUMN `Level` `Level` SMALLINT UNSIGNED NOT NULL ,
CHANGE COLUMN `Money` `Gold` INT(10) UNSIGNED NOT NULL , RENAME TO  `action_level_up_payment` ;

ALTER TABLE `soul` 
DROP COLUMN `ConversionSoulCount`;

ALTER TABLE `stage` 
DROP COLUMN `EscapeStamina`,
DROP COLUMN `RemainChance`,
DROP COLUMN `Description`,
DROP COLUMN `MainStage`, 
CHANGE COLUMN `StageID` `ID` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `Name` `TextKey` VARCHAR(32) NOT NULL ,
ADD COLUMN `No` SMALLINT UNSIGNED NOT NULL AFTER `ID`,
ADD UNIQUE INDEX `No_EpisodeID_Type_UNIQUE` (`No`, `EpisodeID`, `Type`), RENAME TO `story_stage` ;

DROP TABLE `story`;

ALTER TABLE `training` 
DROP COLUMN `HeldWeek`,
DROP COLUMN `OrderPriority`,
CHANGE COLUMN `BattleID` `ID` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `StartDay` `OpenedAt` INT UNSIGNED NOT NULL,
CHANGE COLUMN `EndDay`  `ClosedAt` INT UNSIGNED NOT NULL,
ADD COLUMN `TextKey` VARCHAR(32) NOT NULL AFTER `ID`,
DROP PRIMARY KEY,
ADD PRIMARY KEY (`ID`);

CREATE TABLE `training_date` (
  `TrainingID` INT UNSIGNED NOT NULL,
  `Day` TINYINT UNSIGNED NOT NULL,
  PRIMARY KEY (`TrainingID`, `Day`));

CREATE TABLE `training_stage` (
  `ID` INT UNSIGNED NOT NULL,
  `TrainingID` INT UNSIGNED NOT NULL,
  `Level` TINYINT UNSIGNED NOT NULL,
  `PlayerLevel` SMALLINT UNSIGNED NOT NULL,
  `BattleID` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE INDEX `TrainingID_Level_UNIQUE` (`TrainingID`, `Level`));

ALTER TABLE `trials` 
DROP COLUMN `HeldWeek`,
DROP COLUMN `OrderPriority`,
CHANGE COLUMN `TrialsID` `ID` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `Description` `TextKey` VARCHAR(32) NOT NULL ,
CHANGE COLUMN `StartDay` `OpenedAt` INT UNSIGNED NOT NULL,
CHANGE COLUMN `EndDay`  `ClosedAt` INT UNSIGNED NOT NULL,
DROP PRIMARY KEY,
ADD PRIMARY KEY (`ID`), RENAME TO  `trial` ;

CREATE TABLE `trial_date` (
  `TrialID` INT UNSIGNED NOT NULL,
  `Day` TINYINT UNSIGNED NOT NULL,
  PRIMARY KEY (`TrialID`, `Day`));

ALTER TABLE `trials_level` 
CHANGE COLUMN `TrialsLevelID` `ID` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `TrialsID` `TrialID` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `Level` `Level` TINYINT UNSIGNED NOT NULL ,
CHANGE COLUMN `PlayerLevel` `PlayerLevel` SMALLINT UNSIGNED NOT NULL ,
ADD UNIQUE INDEX `TrialID_Level_UNIQUE` (`TrialID`, `Level`), RENAME TO  `trial_stage` ;


-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
ALTER TABLE `story_chapter` 
DROP COLUMN `No`, 
CHANGE COLUMN `TextKey` `Name` VARCHAR(64) NOT NULL ,
CHANGE COLUMN `PlayerLevel` `Level` INT(10) UNSIGNED NOT NULL ,
ADD COLUMN `Description` VARCHAR(280) NOT NULL,
ADD UNIQUE INDEX `ID_UNIQUE` (`ID`), RENAME TO `chapter` ;

ALTER TABLE `character` 
DROP COLUMN `EvolutionType`;

CREATE TABLE `character_evolution` (
  `CharacterID` int(10) unsigned NOT NULL,
  `EvolutionLevel` tinyint(3) unsigned NOT NULL,
  `Soul` smallint(5) unsigned NOT NULL,
  `NecessaryGold` int(10) unsigned NOT NULL,
  PRIMARY KEY (`CharacterID`,`EvolutionLevel`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

DROP TABLE `character_evolution_type`;

ALTER TABLE `character_gear` 
CHANGE COLUMN `PromotionLevel` `Level` TINYINT UNSIGNED NOT NULL ,
CHANGE COLUMN `GearID1` `Gear1` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `GearID2` `Gear2` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `GearID3` `Gear3` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `GearID4` `Gear4` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `GearID5` `Gear5` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `GearID6` `Gear6` INT(10) UNSIGNED NOT NULL ;

CREATE TABLE `defensive_war` (
  `BattleID` int(10) unsigned NOT NULL,
  `OrderPriority` int(1) unsigned NOT NULL,
  `HeldWeek` int(1) unsigned NOT NULL,
  `StartDay` timestamp NOT NULL,
  `EndDay` timestamp NOT NULL,
  PRIMARY KEY (`BattleID`,`OrderPriority`,`HeldWeek`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

ALTER TABLE `drug` 
CHANGE COLUMN `CharacterExp` `Exp` INT(10) UNSIGNED NOT NULL ;

ALTER TABLE `story_episode` 
DROP COLUMN `No`,
CHANGE COLUMN `ID` `EpisodeID` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `TextKey` `Name` VARCHAR(128) NOT NULL ,
ADD COLUMN `AfterEndEpisode` TINYINT UNSIGNED DEFAULT NULL AFTER `ChapterID` ,
ADD COLUMN `CharacterID1` INT UNSIGNED NOT NULL AFTER `Name` ,
ADD COLUMN `CharacterID2` INT UNSIGNED DEFAULT NULL AFTER `CharacterID1` ,
ADD COLUMN `CharacterID3` INT UNSIGNED DEFAULT NULL AFTER `CharacterID2` ,
DROP INDEX `No_ChapterID_UNIQUE`, RENAME TO `episode` ;

ALTER TABLE `gear` 
CHANGE COLUMN `PhysicalAttack` `PhysicalAttack` FLOAT UNSIGNED NOT NULL ,
CHANGE COLUMN `Armor` `Armor` FLOAT UNSIGNED NOT NULL ,
CHANGE COLUMN `MagicResistance` `MagicResistance` FLOAT UNSIGNED NOT NULL ,
CHANGE COLUMN `PhysicalCrit` `PhysicalCrit` FLOAT UNSIGNED NOT NULL ,
CHANGE COLUMN `MagicPower` `MagicPower` FLOAT UNSIGNED NOT NULL AFTER `PhysicalAttack`, 
CHANGE COLUMN `Strength` `Strength` INT(10) UNSIGNED NOT NULL AFTER `AgilityGrowth`, 
CHANGE COLUMN `Intellect` `Intellect` INT(10) UNSIGNED NOT NULL AFTER `Strength`, 
CHANGE COLUMN `Agility` `Agility` INT(10) UNSIGNED NOT NULL AFTER `Intellect`, 
CHANGE COLUMN `Dodge` `Dodge` FLOAT UNSIGNED NOT NULL AFTER `LifeStealRating`, 
CHANGE COLUMN `ArmorPenetration` `ArmorPenetration` FLOAT UNSIGNED NOT NULL AFTER `Dodge`, 
CHANGE COLUMN `IgnoreMagicResistance` `IgnoreMagicResistance` FLOAT UNSIGNED NOT NULL AFTER `Dodge`, 
CHANGE COLUMN `Accuracy` `Accuracy` FLOAT UNSIGNED NOT NULL AFTER `PhysicalCrit`, 
CHANGE COLUMN `ImproveHealings` `ImproveHealings` FLOAT UNSIGNED NOT NULL AFTER `LifeStealRating`, 
ADD COLUMN `GrowthRate1` float unsigned NOT NULL AFTER `Accuracy`, 
ADD COLUMN `GrowthRate2` float unsigned NOT NULL AFTER `GrowthRate1`, 
ADD COLUMN `GrowthRate3` float unsigned NOT NULL AFTER `GrowthRate2`, 
ADD COLUMN `GrowthRate4` float unsigned NOT NULL AFTER `GrowthRate3`, 
ADD COLUMN `GrowthRate5` float unsigned NOT NULL AFTER `GrowthRate4`, 
ADD COLUMN `Exp1` int(10) unsigned NOT NULL AFTER `GrowthRate5`, 
ADD COLUMN `Exp2` int(10) unsigned NOT NULL AFTER `Exp1`, 
ADD COLUMN `Exp3` int(10) unsigned NOT NULL AFTER `Exp2`, 
ADD COLUMN `Exp4` int(10) unsigned NOT NULL AFTER `Exp3`, 
ADD COLUMN `Exp5` int(10) unsigned NOT NULL AFTER `Exp4`, 
ADD COLUMN `Gold1` int(10) unsigned NOT NULL AFTER `Exp5`, 
ADD COLUMN `Gold2` int(10) unsigned NOT NULL AFTER `Gold1`, 
ADD COLUMN `Gold3` int(10) unsigned NOT NULL AFTER `Gold2`, 
ADD COLUMN `Gold4` int(10) unsigned NOT NULL AFTER `Gold3`, 
ADD COLUMN `Gold5` int(10) unsigned NOT NULL AFTER `Gold4`, 
ADD COLUMN `Gem` int(10) unsigned NOT NULL AFTER `Gold5`, 
ADD COLUMN `EnchantExp` int(10) unsigned NOT NULL AFTER `Gem`; 

DROP TABLE `gear_enchant_exp` ;

DROP TABLE `gear_enchant_level` ;

ALTER TABLE `gear_enchant_material` 
CHANGE COLUMN `EnchantExp` `Exp` INT(10) UNSIGNED NOT NULL ;

ALTER TABLE `gear_piece` 
CHANGE COLUMN `EnchantExp` `Exp` INT(10) UNSIGNED NOT NULL ;

ALTER TABLE `gear_recipe` 
CHANGE COLUMN `MaterialType` `MaterialType` INT(1) UNSIGNED NOT NULL, 
ADD UNIQUE INDEX `ID_UNIQUE` (`ID` ASC);

ALTER TABLE `item` 
ADD COLUMN `FlavorText` varchar(208) NOT NULL,
CHANGE COLUMN `TextKey` `Name` VARCHAR(64) NOT NULL ,
ADD UNIQUE INDEX `ID_UNIQUE` (`ID`);

CREATE TABLE `raid_ticket` (
  `ID` int(10) unsigned NOT NULL,
  `PlayCount` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci ;

ALTER TABLE `action_level_up_payment` 
CHANGE COLUMN `Level` `Level` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `Gold` `Money` INT(10) UNSIGNED NOT NULL , RENAME TO  `skill_level_price` ;

ALTER TABLE `soul` 
ADD COLUMN `ConversionSoulCount` SMALLINT UNSIGNED NOT NULL AFTER `CharacterID`;
ALTER TABLE `story_stage` 
DROP COLUMN `No` ,
CHANGE COLUMN `ID` `StageID` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `TextKey` `Name` VARCHAR(64) NOT NULL ,
ADD COLUMN `Description` VARCHAR(128) NOT NULL AFTER `Name` ,
ADD COLUMN `RemainChance` TINYINT UNSIGNED NOT NULL AFTER `Description` ,
ADD COLUMN `EscapeStamina` SMALLINT UNSIGNED NOT NULL AFTER `RemainChance` ,
ADD COLUMN `MainStage` SMALLINT UNSIGNED NOT NULL AFTER `Stamina`,
DROP INDEX `No_EpisodeID_Type_UNIQUE` , RENAME TO  `stage` ;

CREATE TABLE `story` (
  `StoryID` int(10) unsigned NOT NULL,
  `Order` int(2) unsigned NOT NULL,
  `CharacterID` int(10) unsigned NOT NULL,
  `Serif` varchar(280) NOT NULL,
  PRIMARY KEY (`StoryID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

ALTER TABLE `training` 
DROP COLUMN `TextKey`,
CHANGE COLUMN `ID` `BattleID` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `OpenedAt` `StartDay` INT UNSIGNED NOT NULL,
CHANGE COLUMN `ClosedAt` `EndDay` INT UNSIGNED NOT NULL,
ADD COLUMN `OrderPriority` INT(1) UNSIGNED NOT NULL ,
ADD COLUMN `HeldWeek` INT(1) UNSIGNED NOT NULL ,
DROP PRIMARY KEY,
ADD PRIMARY KEY (`BattleID`, `OrderPriority`, `HeldWeek`);

DROP TABLE `training_date`;

DROP TABLE `training_stage`;

ALTER TABLE `trial` 
CHANGE COLUMN `ID` `TrialsID` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `TextKey` `Description` TEXT NOT NULL ,
CHANGE COLUMN `OpenedAt` `StartDay` INT UNSIGNED NOT NULL,
CHANGE COLUMN `ClosedAt` `EndDay` INT UNSIGNED NOT NULL,
ADD COLUMN `OrderPriority` INT(1) UNSIGNED NOT NULL ,
ADD COLUMN `EndDay` INT(1) UNSIGNED NOT NULL ,
DROP PRIMARY KEY,
ADD PRIMARY KEY (`TrialsID`, `OrderPriority`, `HeldWeek`, `StartDay`, `EndDay`), RENAME TO  `trials` ;

DROP TABLE `trial_date`;

ALTER TABLE `trial_stage` 
CHANGE COLUMN `ID` `TrialsLevelID` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `TrialID` `TrialsID` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `Level` `Level` INT(1) UNSIGNED NOT NULL ,
CHANGE COLUMN `PlayerLevel` `PlayerLevel` INT(2) UNSIGNED NOT NULL ,
DROP INDEX `TrialID_Level_UNIQUE`, RENAME TO  `trials_level` ;
