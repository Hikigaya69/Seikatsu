import { Link } from "react-router-dom";

function Navbar() {
    return (
        <nav className="relative h-[70px] overflow-hidden">

          
            <img
                src="/GIF/wildlife mt GIF.gif"
                alt="bg"
                className="absolute inset-0 w-full h-full object-cover"
            />

            
            <div className="absolute inset-0 bg-black/40 backdrop-blur-sm"></div>

           
            <div className="relative z-10 flex items-center justify-between px-6 h-full text-white">

                {/* Logo */}
                <div className="flex items-center gap-2">
                    <div className="bg-rose-500 text-black w-9 h-9 flex items-center justify-center rounded-lg  font-bold">
                        S
                    </div>
                    <span className="font-semibold text-xl tracking-wide">
                        Seikatsu
                    </span>
                </div>

                {/* Links */}
                <div className="flex items-center gap-8">

                    <Link
                        to="/home"
                        className="font-semibold tracking-wide relative group"
                    >
                        Explore
                        <span className="absolute left-0 bottom-0 w-0 h-[2px] bg-white transition-all duration-300 group-hover:w-full"></span>
                    </Link>

                    <Link
                        to="/login"
                        className="font-semibold tracking-wide relative group"
                    >
                        Login
                        <span className="absolute left-0 bottom-0 w-0 h-[2px] bg-white transition-all duration-300 group-hover:w-full"></span>
                    </Link>

                    <Link
                        to="/register"
                        className="font-semibold tracking-wide relative group"
                    >
                        Register
                        <span className="absolute left-0 bottom-0 w-0 h-[2px] bg-white transition-all duration-300 group-hover:w-full"></span>
                    </Link>

                </div>
            </div>

        </nav>
    );
}

export default Navbar;