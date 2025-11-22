-- MySQL SQL Dump
-- Compatible with MySQL 8.x
-- Generated on: 2025-11-12

-- Create database
CREATE DATABASE IF NOT EXISTS sp_fms;
USE sp_fms;

-- Table structure for `students`
CREATE TABLE IF NOT EXISTS students (
  id VARCHAR(20) NOT NULL,
  first_name VARCHAR(50) NOT NULL,
  last_name VARCHAR(50) NOT NULL,
  course VARCHAR(50) NOT NULL,
  email VARCHAR(100) NOT NULL,
  contact VARCHAR(20) NOT NULL,
  password VARCHAR(255) NOT NULL,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Insert sample data
INSERT INTO students (id, first_name, last_name, course, email, contact, password) VALUES
('24-1554', 'Test', 'Test', 'SFIT - 2B', 'test@gmail.com', '09145215472', '123'),
('24-1884', 'Jehd Ralph', 'Calilan', 'SFIT - 2B', 'calilan@gmail.com', '09947673672', '123');

-- Table structure for `todo_tasks`
CREATE TABLE IF NOT EXISTS todo_tasks (
    task_id INT AUTO_INCREMENT PRIMARY KEY,
    student_id VARCHAR(20),
    task_name VARCHAR(255),
    is_completed TINYINT(1) DEFAULT 0,
    FOREIGN KEY (student_id) REFERENCES students(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Table structure for `todo_progress`
CREATE TABLE IF NOT EXISTS todo_progress (
    progress_id INT AUTO_INCREMENT PRIMARY KEY,
    student_id VARCHAR(20),
    date_recorded DATE,
    completed_tasks INT,
    total_tasks INT,
    FOREIGN KEY (student_id) REFERENCES students(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Example: Insert progress for a student
-- 1. Count completed tasks
-- 2. Count total tasks
-- 3. Calculate completion percentage
-- 4. Insert into todo_progress

-- Replace @id with the student's id
SET @id = '24-1554';

INSERT INTO todo_progress (student_id, date_recorded, completed_tasks, total_tasks)
SELECT
    @id AS student_id,
    CURDATE() AS date_recorded,
    SUM(CASE WHEN is_completed = 1 THEN 1 ELSE 0 END) AS completed_tasks,
    COUNT(*) AS total_tasks
FROM todo_tasks
WHERE student_id = @id;


--
-- Table structure for table `students`
--

-- Example: Get last 7 records of progress for a student
SELECT completed_tasks, total_tasks, date_recorded
FROM todo_progress
WHERE student_id = @id
ORDER BY date_recorded DESC
LIMIT 7;

--
-- Dumping data for table `students`
--

INSERT INTO `students` (`id`, `first_name`, `last_name`, `course`, `email`, `contact`, `password`) VALUES
('24-1554', 'Test', 'Test', 'SFIT - 2B', 'test@gmail.com', '09145215472', '123'),
('24-1884', 'Jehd Ralph', 'Calilan', 'SFIT - 2B', 'calilan@gmail.com', '09947673672', '123');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `students`
--
ALTER TABLE `students`
  ADD PRIMARY KEY (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
