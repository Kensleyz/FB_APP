import { ChevronLeft, ChevronRight } from 'lucide-react';
import type { ScheduleDto } from '../../types/dashboard';

interface CalendarProps {
  month: Date;
  posts: ScheduleDto[];
  onMonthChange: (date: Date) => void;
  onDayClick: (date: Date) => void;
}

export function Calendar({ month, posts, onMonthChange, onDayClick }: CalendarProps) {
  const year = month.getFullYear();
  const monthIdx = month.getMonth();
  const firstDay = new Date(year, monthIdx, 1).getDay();
  const daysInMonth = new Date(year, monthIdx + 1, 0).getDate();
  const today = new Date();

  const days = Array.from({ length: 42 }, (_, i) => {
    const day = i - firstDay + 1;
    if (day < 1 || day > daysInMonth) return null;
    return day;
  });

  const getPostsForDay = (day: number) => {
    return posts.filter((p) => {
      const d = new Date(p.scheduledFor);
      return d.getDate() === day && d.getMonth() === monthIdx && d.getFullYear() === year;
    });
  };

  const prevMonth = () => onMonthChange(new Date(year, monthIdx - 1, 1));
  const nextMonth = () => onMonthChange(new Date(year, monthIdx + 1, 1));

  const monthLabel = month.toLocaleDateString('en-ZA', { month: 'long', year: 'numeric' });

  const isToday = (day: number) =>
    day === today.getDate() && monthIdx === today.getMonth() && year === today.getFullYear();

  return (
    <div className="bg-white rounded-xl border border-gray-200 shadow-sm">
      <div className="flex items-center justify-between p-4 border-b border-gray-200">
        <button onClick={prevMonth} className="p-1.5 hover:bg-gray-100 rounded-lg">
          <ChevronLeft className="w-5 h-5 text-gray-600" />
        </button>
        <h3 className="text-lg font-semibold text-gray-900">{monthLabel}</h3>
        <button onClick={nextMonth} className="p-1.5 hover:bg-gray-100 rounded-lg">
          <ChevronRight className="w-5 h-5 text-gray-600" />
        </button>
      </div>

      <div className="grid grid-cols-7">
        {['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'].map((d) => (
          <div key={d} className="p-2 text-center text-xs font-medium text-gray-500 border-b border-gray-100">
            {d}
          </div>
        ))}

        {days.map((day, i) => {
          if (day === null) {
            return <div key={i} className="p-2 min-h-[80px] border-b border-r border-gray-50" />;
          }

          const dayPosts = getPostsForDay(day);

          return (
            <div
              key={i}
              onClick={() => onDayClick(new Date(year, monthIdx, day))}
              className="p-2 min-h-[80px] border-b border-r border-gray-50 cursor-pointer hover:bg-gray-50 transition-colors"
            >
              <span
                className={`inline-flex items-center justify-center w-7 h-7 text-sm rounded-full ${
                  isToday(day)
                    ? 'bg-primary-600 text-white font-bold'
                    : 'text-gray-700'
                }`}
              >
                {day}
              </span>
              <div className="mt-1 space-y-0.5">
                {dayPosts.slice(0, 2).map((p) => (
                  <div
                    key={p.id}
                    className={`text-xs px-1.5 py-0.5 rounded truncate ${
                      p.status === 'Published'
                        ? 'bg-green-100 text-green-700'
                        : p.status === 'Failed'
                        ? 'bg-red-100 text-red-700'
                        : 'bg-primary-100 text-primary-700'
                    }`}
                  >
                    {p.content.slice(0, 20)}
                  </div>
                ))}
                {dayPosts.length > 2 && (
                  <p className="text-xs text-gray-400 px-1.5">
                    +{dayPosts.length - 2} more
                  </p>
                )}
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}
