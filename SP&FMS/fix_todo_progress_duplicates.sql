-- SQL script to fix todo_progress duplicates
-- Run this script in your MySQL database to prevent duplicate entries

USE sp_fms;

-- Add completion_date column to todo_tasks if it doesn't exist
-- This allows tracking when tasks were completed for automatic cleanup
-- Note: Run this only if the column doesn't exist, otherwise you'll get an error
-- Check first: SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'todo_tasks' AND COLUMN_NAME = 'completion_date';

-- If column doesn't exist, run this:
-- ALTER TABLE todo_tasks ADD COLUMN completion_date DATE NULL;

-- Add unique constraint to todo_progress to prevent duplicate entries per student per day
-- This ensures only one progress record exists per student per date
ALTER TABLE todo_progress
ADD UNIQUE KEY unique_student_date (student_id, date_recorded);

-- Note: If you get an error that the unique key already exists, that's fine.
-- If you get an error about duplicate entries, you may need to clean up existing duplicates first:
-- DELETE t1 FROM todo_progress t1
-- INNER JOIN todo_progress t2 
-- WHERE t1.progress_id > t2.progress_id 
-- AND t1.student_id = t2.student_id 
-- AND t1.date_recorded = t2.date_recorded;

