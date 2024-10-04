-- MySQL Administrator dump 1.4
--
-- ------------------------------------------------------
-- Server version	5.0.4-beta-nt


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


--
-- Create schema testframework
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ testframework;
USE testframework;

--
-- Table structure for table `testframework`.`customer`
--

DROP TABLE IF EXISTS `customer`;
CREATE TABLE `customer` (
  `id` int(10) unsigned NOT NULL auto_increment,
  `phone` varchar(45) NOT NULL,
  `money` double NOT NULL,
  `deleted` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY  (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `testframework`.`customer`
--

/*!40000 ALTER TABLE `customer` DISABLE KEYS */;
/*!40000 ALTER TABLE `customer` ENABLE KEYS */;


--
-- Table structure for table `testframework`.`customer_1`
--

DROP TABLE IF EXISTS `customer_1`;
CREATE TABLE `customer_1` (
  `Id` int(10) unsigned NOT NULL auto_increment,
  `Name` varchar(100) default NULL,
  `Address` varchar(255) default NULL,
  `Phone` varchar(20) default NULL,
  `money` double NOT NULL,
  `deleted` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `testframework`.`customer_1`
--

/*!40000 ALTER TABLE `customer_1` DISABLE KEYS */;
/*!40000 ALTER TABLE `customer_1` ENABLE KEYS */;


--
-- Table structure for table `testframework`.`customer_lang`
--

DROP TABLE IF EXISTS `customer_lang`;
CREATE TABLE `customer_lang` (
  `Lid` int(10) unsigned NOT NULL,
  `name` varchar(100) NOT NULL,
  `address` varchar(100) NOT NULL,
  `LanguageKey` varchar(45) NOT NULL,
  PRIMARY KEY  (`Lid`,`LanguageKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `testframework`.`customer_lang`
--

/*!40000 ALTER TABLE `customer_lang` DISABLE KEYS */;
/*!40000 ALTER TABLE `customer_lang` ENABLE KEYS */;


--
-- Table structure for table `testframework`.`repo`
--

DROP TABLE IF EXISTS `repo`;
CREATE TABLE `repo` (
  `Id` int(10) unsigned NOT NULL auto_increment,
  `CusId` int(10) unsigned default NULL,
  `Are` double default NULL,
  `Name` varchar(45) default NULL,
  `Address` varchar(255) default NULL,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `testframework`.`repo`
--

/*!40000 ALTER TABLE `repo` DISABLE KEYS */;
/*!40000 ALTER TABLE `repo` ENABLE KEYS */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
