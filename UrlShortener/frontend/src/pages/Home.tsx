import { Link } from 'react-router-dom';

const Home = () => {
  return (
    <div className="min-h-screen bg-slate-950 flex flex-col items-center justify-center text-white relative overflow-hidden">
      {/* Background gradients */}
      <div className="absolute top-0 left-1/2 -translate-x-1/2 w-[800px] h-[500px] bg-indigo-600/30 blur-[120px] rounded-full pointer-events-none" />
      
      <div className="z-10 text-center space-y-6 max-w-3xl px-4">
        <h1 className="text-5xl md:text-7xl font-extrabold tracking-tight bg-clip-text text-transparent bg-gradient-to-r from-indigo-400 to-cyan-400">
          LinkForge
        </h1>
        <p className="text-xl md:text-2xl text-slate-300">
          Premium URL shortening with advanced analytics and access control.
        </p>
        <div className="flex flex-col sm:flex-row gap-4 justify-center pt-8">
          <Link
            to="/register"
            className="px-8 py-4 bg-indigo-600 hover:bg-indigo-500 transition-all rounded-xl font-semibold text-lg shadow-[0_0_20px_rgba(79,70,229,0.4)]"
          >
            Get Started
          </Link>
          <Link
            to="/login"
            className="px-8 py-4 bg-slate-800/50 hover:bg-slate-700/50 backdrop-blur-md border border-slate-700 transition-all rounded-xl font-semibold text-lg"
          >
            Sign In
          </Link>
        </div>
      </div>
    </div>
  );
};

export default Home;
