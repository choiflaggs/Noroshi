
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

CREATE TABLE `action` (
  `ID` int(10) unsigned NOT NULL,
  `ClassID` int(10) unsigned NOT NULL,
  `TriggerID` tinyint(3) unsigned NOT NULL,
  `Name` text NOT NULL,
  `Description` text NOT NULL,
  `DamageType` tinyint(4) NOT NULL,
  `DamagePhysicalAttribute` tinyint(4) NOT NULL,
  `DamageMagicalAttribute` tinyint(4) NOT NULL,
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
  `HitSoundID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`)
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
  `FieldID` int(10) unsigned NOT NULL,
  `Exp` int(10) unsigned NOT NULL,
  `Money` int(10) unsigned NOT NULL,
  `GachaID` int(10) unsigned NOT NULL,
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
  `Description` varchar(280) NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `character_action_sequence` (
  `CharacterID` int(10) unsigned NOT NULL,
  `TargetActionNum` tinyint(3) unsigned NOT NULL,
  `SecondLoopStartPosition` tinyint(3) unsigned NOT NULL,
  `ActionSequence1` tinyint(4) NOT NULL,
  `ActionSequence2` tinyint(4) NOT NULL,
  `ActionSequence3` tinyint(4) NOT NULL,
  `ActionSequence4` tinyint(4) NOT NULL,
  `ActionSequence5` tinyint(4) NOT NULL,
  `ActionSequence6` tinyint(4) NOT NULL,
  `ActionSequence7` tinyint(4) NOT NULL,
  PRIMARY KEY (`CharacterID`,`TargetActionNum`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `character_effect` (
  `ID` int(10) unsigned NOT NULL,
  `PrefabID` int(10) unsigned NOT NULL,
  `AnimationName` varchar(64) NOT NULL,
  `MultiAnimation` tinyint(3) unsigned NOT NULL,
  `HasText` tinyint(4) NOT NULL,
  `OrderInCharacterLayer` smallint(6) NOT NULL,
  `Position` tinyint(3) unsigned NOT NULL,
  `FixedRotationY` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `character_evolution` (
  `CharacterID` int(10) unsigned NOT NULL,
  `EvolutionLevel` int(3) unsigned NOT NULL,
  `Soul` int(5) unsigned NOT NULL,
  PRIMARY KEY (`CharacterID`,`EvolutionLevel`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
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
  PRIMARY KEY (`CharacterID`,`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `characters` (
  `ID` int(10) unsigned NOT NULL,
  `TagFlags` text NOT NULL,
  `Name` text NOT NULL,
  `Description` text NOT NULL,
  `InitialEvolutionLevel` tinyint(3) unsigned NOT NULL,
  `Position` tinyint(3) unsigned NOT NULL,
  `OrderPriority` int(10) unsigned NOT NULL,
  `OrderInLayer` int(10) unsigned NOT NULL,
  `Type` tinyint(3) unsigned NOT NULL,
  `Strength` float unsigned NOT NULL,
  `Intellect` float unsigned NOT NULL,
  `Agility` float unsigned NOT NULL,
  `StrengthGrowth` float unsigned NOT NULL,
  `IntellectGrowth` float unsigned NOT NULL,
  `AgilityGrowth` float unsigned NOT NULL,
  `MagicCrit` int(10) unsigned NOT NULL,
  `ArmorPenetration` int(10) unsigned NOT NULL,
  `IgnoreMagicResistance` int(10) unsigned NOT NULL,
  `Accuracy` tinyint(3) unsigned NOT NULL,
  `Dodge` tinyint(3) unsigned NOT NULL,
  `HPRegen` int(10) unsigned NOT NULL,
  `EnergyRegen` smallint(5) unsigned NOT NULL,
  `ImproveHealings` tinyint(3) unsigned NOT NULL,
  `ReduceEnergyCost` tinyint(3) unsigned NOT NULL,
  `LifeStealRating` int(10) unsigned NOT NULL,
  `ActionID0` int(10) unsigned NOT NULL,
  `ActionID1` int(10) unsigned NOT NULL,
  `ActionID2` int(10) unsigned NOT NULL,
  `ActionID3` int(10) unsigned NOT NULL,
  `ActionID4` int(10) unsigned NOT NULL,
  `ActionID5` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
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
CREATE TABLE `cpu_battle_story` (
  `ID` int(10) unsigned NOT NULL,
  `BattleID` int(10) unsigned NOT NULL,
  `TriggerID` tinyint(3) unsigned NOT NULL,
  `DramaType` tinyint(4) NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `i1` (`BattleID`,`TriggerID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `cpu_battle_story_message` (
  `StoryID` int(10) unsigned NOT NULL,
  `No` tinyint(3) unsigned NOT NULL,
  `OwnCharacterID` int(10) unsigned NOT NULL,
  `EnemyCharacterID` int(10) unsigned NOT NULL,
  `Content` text NOT NULL,
  PRIMARY KEY (`StoryID`,`No`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `daily_quest` (
  `ID` int(10) unsigned NOT NULL,
  `TriggerID` tinyint(3) unsigned NOT NULL,
  `Threshold` int(10) unsigned NOT NULL,
  `Name` text NOT NULL,
  `Description` text NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `daily_quest_reward` (
  `DailyQuestID` int(10) unsigned NOT NULL,
  `No` tinyint(3) unsigned NOT NULL,
  `PossessionCategory` tinyint(3) unsigned NOT NULL,
  `PossessionID` int(10) unsigned NOT NULL,
  `PossessionNum` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DailyQuestID`,`No`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `defensive_war` (
  `BattleID` int(10) unsigned NOT NULL,
  `OrderPriority` int(1) unsigned NOT NULL,
  `HeldWeek` int(1) unsigned NOT NULL,
  `StartDay` timestamp NOT NULL,
  `EndDay` timestamp NOT NULL,
  PRIMARY KEY (`BattleID`,`OrderPriority`,`HeldWeek`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `drug` (
  `ItemID` int(10) unsigned NOT NULL,
  `Type` int(1) NOT NULL,
  `Exp` int(11) NOT NULL,
  PRIMARY KEY (`ItemID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
CREATE TABLE `enemy_character` (
  `ID` int(10) unsigned NOT NULL,
  `CharacterID` int(10) unsigned NOT NULL,
  `Level` int(10) unsigned NOT NULL,
  `PromotionLevel` int(10) unsigned NOT NULL,
  `EvolutionLevel` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `episode` (
  `EpisodeID` int(10) unsigned NOT NULL,
  `ChapterID` int(10) unsigned NOT NULL,
  `AfterEndEpisode` int(1) unsigned NOT NULL,
  `Name` varchar(128) NOT NULL,
  `CharacterID1` int(10) unsigned NOT NULL,
  `CharacterID2` int(10) unsigned DEFAULT '0',
  `CharacterID3` int(10) unsigned DEFAULT '0',
  PRIMARY KEY (`EpisodeID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `expedition_stage` (
  `ID` int(10) unsigned NOT NULL,
  `ExpeditionID` int(10) unsigned NOT NULL,
  `Step` tinyint(4) NOT NULL,
  `MaxPlayerLevel` smallint(5) unsigned NOT NULL,
  `MinPlayerLevel` smallint(5) unsigned NOT NULL,
  `Money` int(10) unsigned NOT NULL,
  `GachaID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `i1` (`ExpeditionID`,`Step`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `gacha` (
  `ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `gacha_possession_content` (
  `ContentID` int(10) unsigned NOT NULL,
  `GachaID` int(10) unsigned NOT NULL,
  `PossessionCategory` tinyint(3) unsigned NOT NULL,
  `PossessionID` int(10) unsigned NOT NULL,
  `PossessionNum` int(10) unsigned NOT NULL,
  `Weight` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ContentID`),
  UNIQUE KEY `i1` (`GachaID`,`PossessionCategory`,`PossessionID`,`PossessionNum`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `gear` (
  `ID` int(10) unsigned NOT NULL,
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
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ItemID_UNIQUE` (`ID`)
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
CREATE TABLE `login_bonus` (
  `ID` int(10) unsigned NOT NULL,
  `Name` text NOT NULL,
  `OpenedAt` int(10) unsigned NOT NULL,
  `ClosedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `login_bonus_reward` (
  `ID` int(10) unsigned NOT NULL,
  `LoginBonusID` int(10) unsigned NOT NULL,
  `Threshold` tinyint(3) unsigned NOT NULL,
  `PossessionCategory` tinyint(3) unsigned NOT NULL,
  `PossessionID` int(10) unsigned NOT NULL,
  `PossessionNum` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `i1` (`LoginBonusID`,`Threshold`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player` (
  `ID` int(10) unsigned NOT NULL,
  `UDID` varchar(128) NOT NULL,
  `SessionID` varchar(128) NOT NULL,
  `ShardID` int(10) unsigned NOT NULL,
  `CreatedAt` int(10) unsigned NOT NULL,
  `UpdatedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `i1` (`UDID`),
  UNIQUE KEY `i2` (`SessionID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player_exp` (
  `Level` int(10) unsigned NOT NULL,
  `Exp` int(10) unsigned NOT NULL,
  PRIMARY KEY (`Level`),
  UNIQUE KEY `level_UNIQUE` (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player_vip_exp` (
  `Level` int(10) unsigned NOT NULL,
  `Exp` int(10) unsigned NOT NULL,
  PRIMARY KEY (`Level`),
  UNIQUE KEY `level_UNIQUE` (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `quest` (
  `ID` int(10) unsigned NOT NULL,
  `TriggerID` tinyint(3) unsigned NOT NULL,
  `Threshold` int(10) unsigned NOT NULL,
  `Name` text NOT NULL,
  `Description` text NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `i1` (`TriggerID`,`Threshold`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `quest_reward` (
  `QuestID` int(10) unsigned NOT NULL,
  `No` tinyint(3) unsigned NOT NULL,
  `PossessionCategory` tinyint(3) unsigned NOT NULL,
  `PossessionID` int(10) unsigned NOT NULL,
  `PossessionNum` int(10) unsigned NOT NULL,
  PRIMARY KEY (`QuestID`,`No`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `release_level` (
  `ID` int(10) unsigned NOT NULL,
  `ContentsID` int(10) unsigned NOT NULL,
  `Level` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
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
CREATE TABLE `shop` (
  `ID` int(10) unsigned NOT NULL,
  `MerchandiseNum` tinyint(3) unsigned NOT NULL,
  `Name` text NOT NULL,
  `Description` text NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `shop_merchandise` (
  `ID` int(10) unsigned NOT NULL,
  `ShopID` int(10) unsigned NOT NULL,
  `MaxPlayerLevel` smallint(5) unsigned NOT NULL,
  `MinPlayerLevel` smallint(5) unsigned NOT NULL,
  `MerchandisePossessionCategory` tinyint(3) unsigned NOT NULL,
  `MerchandisePossessionID` int(10) unsigned NOT NULL,
  `MerchandisePossessionNum` int(10) unsigned NOT NULL,
  `PaymentPossessionCategory` tinyint(3) unsigned NOT NULL,
  `PaymentPossessionID` int(10) unsigned NOT NULL,
  `PaymentPossessionNum` int(10) unsigned NOT NULL,
  `Weight` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `i1` (`ShopID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `skill_level_price` (
  `Level` int(10) unsigned NOT NULL,
  `Money` int(10) unsigned NOT NULL,
  PRIMARY KEY (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `soul` (
  `ID` int(10) unsigned NOT NULL,
  `ItemID` int(10) unsigned NOT NULL,
  `Soul` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`,`ItemID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`),
  UNIQUE KEY `new_tablecol_UNIQUE` (`ItemID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `stage` (
  `StageID` int(10) unsigned NOT NULL,
  `EpisodeID` int(10) unsigned NOT NULL,
  `Type` int(1) unsigned NOT NULL,
  `BattleID` int(10) unsigned NOT NULL,
  `Name` varchar(64) NOT NULL,
  `Description` varchar(128) NOT NULL,
  `Stamina` int(10) unsigned NOT NULL,
  `RemainChance` int(3) unsigned NOT NULL,
  `EscapeStamina` int(10) unsigned NOT NULL,
  `MainStage` int(1) unsigned NOT NULL,
  PRIMARY KEY (`StageID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `story` (
  `StoryID` int(10) unsigned NOT NULL,
  `Order` int(2) unsigned NOT NULL,
  `CharacterID` int(10) unsigned NOT NULL,
  `Serif` varchar(280) NOT NULL,
  PRIMARY KEY (`StoryID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `time_debug` (
  `HostName` varchar(128) NOT NULL,
  `ModifyTime` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`HostName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `training` (
  `BattleID` int(10) unsigned NOT NULL,
  `OrderPriority` int(1) unsigned NOT NULL,
  `HeldWeek` int(1) unsigned NOT NULL,
  `StartDay` timestamp NOT NULL,
  `EndDay` timestamp NOT NULL,
  PRIMARY KEY (`BattleID`,`OrderPriority`,`HeldWeek`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `trials` (
  `TrialsID` int(10) unsigned NOT NULL,
  `Description` text NOT NULL,
  `OrderPriority` int(1) unsigned NOT NULL,
  `HeldWeek` int(1) unsigned NOT NULL,
  `StartDay` timestamp NOT NULL,
  `EndDay` timestamp NOT NULL,
  PRIMARY KEY (`TrialsID`,`OrderPriority`,`HeldWeek`,`StartDay`,`EndDay`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `trials_level` (
  `TrialsLevelID` int(10) unsigned NOT NULL,
  `TrialsID` int(10) unsigned NOT NULL,
  `Level` int(2) unsigned NOT NULL,
  `PlayerLevel` int(3) unsigned NOT NULL,
  `BattleID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`TrialsLevelID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `sequential_id_player` (
  `ID` int(10) unsigned NOT NULL
) ENGINE=MyISAM;
INSERT INTO sequential_id_player VALUES (0);

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `action`;
DROP TABLE `attribute`;
DROP TABLE `battle`;
DROP TABLE `battle_drop`;
DROP TABLE `battle_wave`;
DROP TABLE `chapter`;
DROP TABLE `character_action_sequence`;
DROP TABLE `character_effect`;
DROP TABLE `character_evolution`;
DROP TABLE `character_exp`;
DROP TABLE `character_gear`;
DROP TABLE `characters`;
DROP TABLE `cooperate_character`;
DROP TABLE `cooperate_level`;
DROP TABLE `cpu_battle_story`;
DROP TABLE `cpu_battle_story_message`;
DROP TABLE `daily_quest`;
DROP TABLE `daily_quest_reward`;
DROP TABLE `defensive_war`;
DROP TABLE `drug`;
DROP TABLE `enemy_character`;
DROP TABLE `episode`;
DROP TABLE `expedition_stage`;
DROP TABLE `gacha`;
DROP TABLE `gacha_possession_content`;
DROP TABLE `gear`;
DROP TABLE `gear_recipe`;
DROP TABLE `item`;
DROP TABLE `login_bonus`;
DROP TABLE `login_bonus_reward`;
DROP TABLE `player`;
DROP TABLE `player_exp`;
DROP TABLE `player_vip_exp`;
DROP TABLE `quest`;
DROP TABLE `quest_reward`;
DROP TABLE `release_level`;
DROP TABLE `shadow_character`;
DROP TABLE `shop`;
DROP TABLE `shop_merchandise`;
DROP TABLE `skill_level_price`;
DROP TABLE `soul`;
DROP TABLE `stage`;
DROP TABLE `story`;
DROP TABLE `time_debug`;
DROP TABLE `training`;
DROP TABLE `trials`;
DROP TABLE `trials_level`;
DROP TABLE `sequential_id_player`;

