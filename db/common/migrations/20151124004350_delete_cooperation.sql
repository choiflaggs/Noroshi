
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

DROP TABLE `cooperation`;
DROP TABLE `cooperation_parameter`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

CREATE TABLE `cooperation` (
  `ID` int(10) unsigned NOT NULL,
  `TextKey` varchar(32) NOT NULL,
  `CharacterID1` int(10) unsigned NOT NULL,
  `CharacterID2` int(10) unsigned NOT NULL,
  `CharacterID3` int(10) unsigned NOT NULL,
  `CharacterID4` int(10) unsigned NOT NULL,
  `CharacterID5` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `i1` (`TextKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `cooperation_parameter` (
  `CooperationID` int(10) unsigned NOT NULL,
  `Level` tinyint(3) unsigned NOT NULL,
  `Strength` float unsigned NOT NULL,
  `Intellect` float unsigned NOT NULL,
  `Agility` float unsigned NOT NULL,
  `PhysicalAttack` int(10) unsigned NOT NULL,
  `MagicPower` int(10) unsigned NOT NULL,
  `Armor` int(10) unsigned NOT NULL,
  `MagicRegistance` int(10) unsigned NOT NULL,
  `Accuracy` tinyint(3) unsigned NOT NULL,
  `Dodge` tinyint(3) unsigned NOT NULL,
  `ActionFrequency` float unsigned NOT NULL,
  PRIMARY KEY (`CooperationID`,`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

