(defun jfh/time->minutes (hours-minutes)
  "convert a string in HH:MM format to integer minutes"
  (let* ((hours-minutes-number (mapcar #'string-to-number (split-string hours-minutes ":"))))
    (+
     (* 60 (car hours-minutes-number))
     (cadr hours-minutes-number))))

(defun jfh/minutes->billable-hours (minutes hourly-rate)
  "convert integer minutes into billable hours"
  (let ((hours (/ minutes 60))
	(remainder-minutes (mod minutes 60)))
    (*
     hourly-rate
     (+
      hours
      (/ remainder-minutes 60.0)))))


(defun jfh/get-hours-column-index (first-row)
    "Find the Hours column title and return the column index"
    (cl-position "Hours" first-row :test #'string-equal))

(defun jfh/report-hours-and-total-billed-hours (table)
  (let* ((hours-column-index (jfh/get-hours-column-index (first table)))
	 (hours
	  (loop
	   for row in (subseq table 1 (- (length table) 1))
	   collect (nth hours-column-index row)))
	 (total-minutes (reduce #'+ (mapcar #'time->minutes hours)))
	 (hours (/ total-minutes 60))
	 (minutes (mod total-minutes 60))
	 (bill-hours (/ total-minutes 60.0))
	 (total-amount (* 105 bill-hours)))
    (format "Hours: %d:%02d\nTotal amount: $%.2f" hours minutes total-amount)))

;; example of how to sum up hours from time-sheet
;; (cl-reduce #'+ (let ((times(mapcar #'symbol-name '( 0:15    
;; 						    0:53   
;; 						    0:15   
;; 						    0:37   
;; 						    0:44
;; 						    0:08   
;; 						    0:57   
;; 						    0:15   
;; 						    0:15
;; 						    0:27
;; 						    0:16
;; 						    0:42
;; 						    0:35
;; 						    0:26
;; 						    0:51
;; 						    0:16
;; 						    0:51
;; 						    2:29
;; 						    0:11
;; 						    0:15))))
;; 		 (mapcar #'(lambda (e)(minutes->billable-hours (time->minutes e) 105) ) times)))

;;; WIP
;; (defun get-time-from-region ()
;;   "gets all the strings formatted as HH:MM in a highlighted region"
;;   (region-beginning)
;;   (let ((time (string-match "\d+:\d\d" )))
;;     ))
