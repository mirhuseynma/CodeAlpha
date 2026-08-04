import LoginForm from '../features/auth/LoginForm';

const Login = () => {
  return (
    <div className="min-h-screen bg-slate-950 flex flex-col items-center justify-center text-white relative overflow-hidden px-4">
      <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] h-[400px] bg-indigo-600/20 blur-[120px] rounded-full pointer-events-none" />
      
      <div className="z-10 w-full">
        <LoginForm />
      </div>
    </div>
  );
};

export default Login;
