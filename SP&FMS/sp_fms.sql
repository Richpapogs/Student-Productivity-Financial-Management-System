-- MySQL SQL Dump
-- Compatible with MySQL 8.x / MariaDB 10.4+
-- Generated from phpMyAdmin on: 2025-11-23

-- Create database
CREATE DATABASE IF NOT EXISTS sp_fms;
USE sp_fms;

-- --------------------------------------------------------
-- Table structure for table `students`
-- --------------------------------------------------------

CREATE TABLE IF NOT EXISTS `students` (
  `id` varchar(20) NOT NULL,
  `first_name` varchar(50) NOT NULL,
  `last_name` varchar(50) NOT NULL,
  `course` varchar(50) NOT NULL,
  `email` varchar(100) NOT NULL,
  `contact` varchar(20) NOT NULL,
  `password` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------
-- Dumping data for table `students`
-- --------------------------------------------------------

INSERT INTO `students` (`id`, `first_name`, `last_name`, `course`, `email`, `contact`, `password`) VALUES
('24-1877', 'Rich', 'Dego', 'SFIT-2B', 'richdego@gmail.com', '09123781312', 'P3htwbN1NNKqdQdmE4WXWw==:o6iWoZsBdcF703Eovo5u5i8SoktxmtQU+71ZfrmYBlg='),
('24-1879', 'Ries', 'Riyas', 'SFIT - 2B', 'Riesyas@gmail.com', '0912313123', 'WBPibTXyZCTBfstVr1kl1g==:T8wBhPpsfUprZYNZqI4OCVFuondB61mtP6//BaEBpts=');

-- --------------------------------------------------------
-- Table structure for table `todo_tasks`
-- --------------------------------------------------------

CREATE TABLE IF NOT EXISTS `todo_tasks` (
  `task_id` int(11) NOT NULL AUTO_INCREMENT,
  `student_id` varchar(20) DEFAULT NULL,
  `task_name` varchar(255) DEFAULT NULL,
  `is_completed` tinyint(1) DEFAULT 0,
  `completion_date` date DEFAULT NULL,
  PRIMARY KEY (`task_id`),
  KEY `student_id` (`student_id`),
  CONSTRAINT `todo_tasks_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------
-- Dumping data for table `todo_tasks`
-- --------------------------------------------------------

INSERT INTO `todo_tasks` (`task_id`, `student_id`, `task_name`, `is_completed`, `completion_date`) VALUES
(24, '24-1879', 'Activity 1', 1, '2025-11-23'),
(27, '24-1879', 'Activity 3', 0, NULL),
(29, '24-1879', 'Activity 4', 0, NULL),
(30, '24-1877', 'Activity 1', 1, '2025-11-23'),
(33, '24-1877', 'Activity 2', 0, NULL),
(34, '24-1877', 'Activity 3', 0, NULL),
(40, '24-1879', 'Activity 5', 1, '2025-11-24');

-- --------------------------------------------------------
-- Table structure for table `todo_progress`
-- --------------------------------------------------------

CREATE TABLE IF NOT EXISTS `todo_progress` (
  `progress_id` int(11) NOT NULL AUTO_INCREMENT,
  `student_id` varchar(20) DEFAULT NULL,
  `date_recorded` date DEFAULT NULL,
  `completed_tasks` int(11) DEFAULT NULL,
  `total_tasks` int(11) DEFAULT NULL,
  PRIMARY KEY (`progress_id`),
  KEY `student_id` (`student_id`),
  CONSTRAINT `todo_progress_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------
-- Dumping data for table `todo_progress`
-- --------------------------------------------------------

INSERT INTO `todo_progress` (`progress_id`, `student_id`, `date_recorded`, `completed_tasks`, `total_tasks`) VALUES
(17, '24-1879', '2025-11-23', 1, 3),
(18, '24-1877', '2025-11-23', 1, 3),
(19, '24-1879', '2025-11-24', 2, 4);

-- --------------------------------------------------------
-- Table structure for table `student_budget`
-- --------------------------------------------------------

CREATE TABLE IF NOT EXISTS `student_budget` (
  `budget_id` int(11) NOT NULL AUTO_INCREMENT,
  `student_id` varchar(20) NOT NULL,
  `budget_amount` decimal(10,2) NOT NULL,
  `date_set` date NOT NULL,
  `budget_remaining` decimal(10,2) NOT NULL DEFAULT 0.00,
  PRIMARY KEY (`budget_id`),
  UNIQUE KEY `unique_student_date` (`student_id`,`date_set`),
  CONSTRAINT `student_budget_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------
-- Dumping data for table `student_budget`
-- --------------------------------------------------------

INSERT INTO `student_budget` (`budget_id`, `student_id`, `budget_amount`, `date_set`, `budget_remaining`) VALUES
(1, '24-1879', 500.00, '2025-11-23', 200.00),
(2, '24-1879', 500.00, '2025-11-24', 150.00);

-- --------------------------------------------------------
-- Table structure for table `expenses`
-- --------------------------------------------------------

CREATE TABLE IF NOT EXISTS `expenses` (
  `expense_id` int(11) NOT NULL AUTO_INCREMENT,
  `student_id` varchar(20) NOT NULL,
  `category` varchar(100) NOT NULL,
  `cost` decimal(10,2) NOT NULL,
  `date_added` date NOT NULL,
  PRIMARY KEY (`expense_id`),
  KEY `student_id` (`student_id`),
  CONSTRAINT `expenses_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------
-- Dumping data for table `expenses`
-- --------------------------------------------------------

INSERT INTO `expenses` (`expense_id`, `student_id`, `category`, `cost`, `date_added`) VALUES
(1, '24-1879', 'Food', 150.00, '2025-11-23'),
(2, '24-1879', 'Transportation', 100.00, '2025-11-23'),
(3, '24-1879', 'School Activity', 50.00, '2025-11-23'),
(5, '24-1879', 'Food', 130.00, '2025-11-24'),
(6, '24-1879', 'Transportation', 120.00, '2025-11-24'),
(8, '24-1879', 'Others', 100.00, '2025-11-24');

-- Set AUTO_INCREMENT values
ALTER TABLE `expenses` AUTO_INCREMENT = 9;
ALTER TABLE `student_budget` AUTO_INCREMENT = 3;
ALTER TABLE `todo_progress` AUTO_INCREMENT = 20;
ALTER TABLE `todo_tasks` AUTO_INCREMENT = 41;
