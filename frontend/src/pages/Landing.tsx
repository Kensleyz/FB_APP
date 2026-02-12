import { Link } from 'react-router-dom';
import { Zap, PenTool, CalendarDays, BarChart3, Check, ArrowRight } from 'lucide-react';
import { Button } from '../components/common/Button';
import { PricingTable } from '../components/billing/PricingTable';

export function Landing() {
  return (
    <div className="min-h-screen bg-white">
      {/* Nav */}
      <nav className="sticky top-0 bg-white/80 backdrop-blur-md border-b border-gray-100 z-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
          <div className="flex items-center gap-2">
            <Zap className="w-7 h-7 text-primary-600" />
            <span className="text-xl font-bold text-gray-900">PageBoost AI</span>
          </div>
          <div className="flex items-center gap-3">
            <Link to="/login">
              <Button variant="ghost" size="sm">Sign In</Button>
            </Link>
            <Link to="/register">
              <Button size="sm">Get Started Free</Button>
            </Link>
          </div>
        </div>
      </nav>

      {/* Hero */}
      <section className="py-16 sm:py-24 lg:py-32">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <h1 className="text-4xl sm:text-5xl lg:text-6xl font-bold text-gray-900 tracking-tight">
            Grow Your Business on{' '}
            <span className="text-primary-600">Facebook</span>
            <br />with AI-Powered Content
          </h1>
          <p className="mt-6 text-lg sm:text-xl text-gray-600 max-w-2xl mx-auto">
            PageBoost AI helps South African small businesses create engaging Facebook posts,
            schedule content, and grow their audience -- all powered by artificial intelligence.
          </p>
          <div className="mt-8 flex flex-col sm:flex-row items-center justify-center gap-3">
            <Link to="/register">
              <Button size="lg">
                Start Free -- No Credit Card
                <ArrowRight className="w-5 h-5 ml-2" />
              </Button>
            </Link>
            <a href="#features">
              <Button variant="outline" size="lg">See How It Works</Button>
            </a>
          </div>
          <p className="mt-4 text-sm text-gray-500">
            Trusted by spaza shops, salons, churches, and small businesses across Mzansi
          </p>
        </div>
      </section>

      {/* Features */}
      <section id="features" className="py-16 bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold text-gray-900">
              Everything You Need to Boost Your Page
            </h2>
            <p className="mt-3 text-gray-600 max-w-xl mx-auto">
              From AI content creation to scheduling, we handle it all so you can focus on running your business.
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            {[
              {
                icon: PenTool,
                title: 'AI Content Creation',
                description:
                  'Generate engaging Facebook posts tailored for your business type. Choose your tone, style, and language -- AI does the rest.',
              },
              {
                icon: CalendarDays,
                title: 'Smart Scheduling',
                description:
                  'Plan your content calendar in advance. Schedule posts for the best times and never miss a day of engagement.',
              },
              {
                icon: BarChart3,
                title: 'Insights & Analytics',
                description:
                  'Track your page performance, engagement rates, and follower growth. Know what content works best.',
              },
            ].map((feature) => (
              <div
                key={feature.title}
                className="bg-white rounded-xl border border-gray-200 p-6 hover:shadow-md transition-shadow"
              >
                <div className="p-3 bg-primary-50 rounded-lg w-fit mb-4">
                  <feature.icon className="w-6 h-6 text-primary-600" />
                </div>
                <h3 className="text-lg font-semibold text-gray-900">{feature.title}</h3>
                <p className="mt-2 text-gray-600 text-sm">{feature.description}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* SA Specific */}
      <section className="py-16">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold text-gray-900">
              Built for South African Businesses
            </h2>
            <p className="mt-3 text-gray-600 max-w-xl mx-auto">
              We understand local business culture, languages, and what makes Mzansi tick.
            </p>
          </div>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
            {[
              'Spaza Shops',
              'Hair Salons',
              'Churches',
              'Restaurants',
              'Gyms',
              'Funeral Parlors',
              'Taxi Associations',
              'General Businesses',
            ].map((type) => (
              <div
                key={type}
                className="flex items-center gap-2 p-4 bg-gray-50 rounded-lg"
              >
                <Check className="w-5 h-5 text-green-500" />
                <span className="text-sm font-medium text-gray-700">{type}</span>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Pricing */}
      <section id="pricing" className="py-16 bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold text-gray-900">Simple, Affordable Pricing</h2>
            <p className="mt-3 text-gray-600">
              Start free. Upgrade when you're ready.
            </p>
          </div>
          <PricingTable onSelectPlan={() => window.location.href = '/register'} />
        </div>
      </section>

      {/* CTA */}
      <section className="py-16">
        <div className="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <h2 className="text-3xl font-bold text-gray-900">
            Ready to Boost Your Facebook Page?
          </h2>
          <p className="mt-3 text-gray-600">
            Join hundreds of South African businesses already using PageBoost AI to grow their audience.
          </p>
          <div className="mt-8">
            <Link to="/register">
              <Button size="lg">
                Get Started Free
                <ArrowRight className="w-5 h-5 ml-2" />
              </Button>
            </Link>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className="border-t border-gray-200 py-8">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex flex-col sm:flex-row items-center justify-between gap-4">
          <div className="flex items-center gap-2">
            <Zap className="w-5 h-5 text-primary-600" />
            <span className="text-sm font-medium text-gray-900">PageBoost AI</span>
          </div>
          <p className="text-sm text-gray-500">
            &copy; {new Date().getFullYear()} PageBoost AI. Made in South Africa.
          </p>
        </div>
      </footer>
    </div>
  );
}
