/*
 Copyright (C) 2008 Siarhei Novik (snovik@gmail.com)
  
 This file is part of QLNet Project http://qlnet.sourceforge.net/

 QLNet is free software: you can redistribute it and/or modify it
 under the terms of the QLNet license.  You should have received a
 copy of the license along with this program; if not, license is  
 available online at <http://qlnet.sourceforge.net/License.html>.
  
 QLNet is a based on QuantLib, a free-software/open-source library
 for financial quantitative analysts and developers - http://quantlib.org/
 The QuantLib license is available online at http://quantlib.org/license.shtml.
 
 This program is distributed in the hope that it will be useful, but WITHOUT
 ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 FOR A PARTICULAR PURPOSE.  See the license for more details.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QLNet {
    //! Main cycle of the International %Money Market (a.k.a. %IMM) months
    public struct IMM {
        enum Month {
            F = 1, G = 2, H = 3,
            J = 4, K = 5, M = 6,
            N = 7, Q = 8, U = 9,
            V = 10, X = 11, Z = 12
        }

        //! returns whether or not the given date is an IMM date
        public static bool isIMMdate(Date date) { return isIMMdate(date, true); }
        public static bool isIMMdate(Date date, bool mainCycle) {
            if (date.DayOfWeek != DayOfWeek.Wednesday)
                return false;

            if (date.Day < 15 || date.Day > 21)
                return false;

            if (!mainCycle) return true;

            switch ((QLNet.Month)date.Month) {
                case QLNet.Month.March:
                case QLNet.Month.June:
                case QLNet.Month.September:
                case QLNet.Month.December:
                    return true;
                default:
                    return false;
            }
        }

        //! returns whether or not the given string is an IMM code
        public static bool isIMMcode(string s) { return isIMMcode(s, true); }
        public static bool isIMMcode(string s, bool mainCycle) {
            if (s.Length != 2)
                return false;

            string str1 = "0123456789";
            if (!str1.Contains(s[1]))
                return false;

            if (mainCycle) str1 = "hmzuHMZU";
            else           str1 = "fghjkmnquvxzFGHJKMNQUVXZ";
            if (!str1.Contains(s[0]))
                return false;

            return true;
        }

        // returns the IMM code for the given date (e.g. H3 for March 20th, 2013).
        public static string code(Date immDate) {
            if (!isIMMdate(immDate, false)) throw new ArgumentException(immDate + " is not an IMM date");
            return "FGHJKMNQUVXZ"[immDate.Month-1] + (immDate.Year % 10).ToString();
        }

        // returns the IMM date for the given IMM code (e.g. March 20th, 2013 for H3).
        public static Date date(string immCode, SavedSettings settings) { return date(immCode, null, settings); }
        public static Date date(string immCode, Date refDate, SavedSettings settings) {
            if (!isIMMcode(immCode, false)) throw new ArgumentException(immCode + " is not a valid IMM code");

            Date referenceDate = (refDate ?? settings.evaluationDate());

            int m = "FGHJKMNQUVXZ".IndexOf(immCode.ToUpper()[0]) + 1;
            if (m == 0)
                throw new ArgumentException("invalid IMM month letter");

            if (!Char.IsDigit(immCode[1]))
                throw new ArgumentException(immCode + " is not a valid IMM code");
            int y = immCode[1] - '0';

            y += referenceDate.Year - (referenceDate.Year % 10);

            /* year<10 are not valid years: to avoid a run-time
               exception in few lines below we need to add 10 years right away */
            if (y == 0 && referenceDate.Year <= 1909) y += 10;

            Date result = IMM.nextDate(new Date(1, m, y), false, settings);
            if (result<referenceDate)
                result = IMM.nextDate(new Date(1, m, y + 10), false, settings);

            return result;
        }

        //! next IMM date following the given date
        /*! returns the 1st delivery date for next contract listed in the
            International Money Market section of the Chicago Mercantile Exchange. */
        public static Date nextDate(SavedSettings settings) { return nextDate((Date)null, true, settings); }
        public static Date nextDate(Date d, SavedSettings settings) { return nextDate(d, true, settings); }
        public static Date nextDate(Date date, bool mainCycle, SavedSettings settings) {
            Date refDate = (date == null ? settings.evaluationDate() : date);

            int y = refDate.Year;
            int m = refDate.Month;

            int offset = mainCycle ? 3 : 1;
            int skipMonths = offset-(m%offset);
            if (skipMonths != offset || refDate.Day > 21) {
                skipMonths += m;
                if (skipMonths<=12) {
                    m = skipMonths;
                } else {
                    m = skipMonths-12;
                    y += 1;
                }
            }

            Date result = Date.nthWeekday(3, DayOfWeek.Wednesday, m, y);
            if (result<=refDate)
                result = nextDate(new Date(22, m, y), mainCycle, settings);
            return result;
        }

        //! next IMM date following the given IMM code
        /*! returns the 1st delivery date for next contract listed in the
            International Money Market section of the Chicago Mercantile Exchange. */
        public static Date nextDate(string immCode, SavedSettings settings) { return nextDate(immCode, true, null, settings); }
        public static Date nextDate(string immCode, bool mainCycle, SavedSettings settings) { return nextDate(immCode, mainCycle, null, settings); }
        public static Date nextDate(string immCode, bool mainCycle, Date referenceDate, SavedSettings settings) {
            Date immDate = date(immCode, referenceDate, settings);
            return nextDate(immDate+1, mainCycle, settings);
        }

        /*! returns the IMM code for next contract listed in the
            International Money Market section of the Chicago Mercantile Exchange.*/
        public static string nextCode(SavedSettings settings) { return nextCode((Date)null, true, settings); }
        public static string nextCode(Date d, SavedSettings settings) { return nextCode(d, true, settings); }
        public static string nextCode(Date d, bool mainCycle, SavedSettings settings) {
            Date date = nextDate(d, mainCycle, settings);
            return code(date);
        }

        /*! returns the IMM code for next contract listed in the
            International Money Market section of the Chicago Mercantile Exchange. */
        public static string nextCode(string immCode, SavedSettings settings) { return nextCode(immCode, true, null, settings); }
        public static string nextCode(string immCode, bool mainCycle, SavedSettings settings) { return nextCode(immCode, mainCycle, null, settings); }
        public static string nextCode(string immCode, bool mainCycle, Date referenceDate, SavedSettings settings) {
            Date date = nextDate(immCode, mainCycle, referenceDate, settings);
            return code(date);
        }
    }
}
