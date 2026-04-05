
import { ShoppingCart, User, LogOut, LogIn } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";

import api from "../../Utils/api";

export default function HomeNavbar() {
    const navigate = useNavigate();
    const [cartCount, setCartCount] = useState(0);
    const [isLoggedIn, setIsLoggedIn] = useState(false);

    useEffect(() => {
        const checkAuth = async () => {
            try {
                await api.get("/Auth/check", { withCredentials: true });
                setIsLoggedIn(true);
            } catch {
                setIsLoggedIn(false);
            }
        };

        const fetchCartSummary = async () => {
            try {
                const res = await api.get("/Cart/cartsummary", {
                    withCredentials: true
                });
                setCartCount(res.data.data.totalItems);
            } catch {
                // not logged in or empty cart
            }
        };

        checkAuth();
        fetchCartSummary();
    }, []);

    const handleLogout = async () => {
        try {
            await api.post("/Auth/logout", {}, { withCredentials: true });
            setIsLoggedIn(false);
            setCartCount(0);
            navigate("/home");
        } catch (err) {
            console.error(err);
        }
    };

    return (
        <div className="bg-white border-b p-2 shadow-sm">
            <div className="max-w-7xl mx-auto flex items-center justify-between p-5">
                <h1
                    onClick={() => navigate("/home")}
                    className="text-2xl font-bold text-[#284b63] cursor-pointer hover:opacity-80 transition"
                >
                    Seikatsu
                </h1>
                <input
                    type="text"
                    placeholder="Search product"
                    className="w-125 px-4 py-2 border rounded-lg"
                />
                <div className="flex gap-6 items-center">
                    {isLoggedIn && (
                        <User className="cursor-pointer" onClick={() => navigate("/profile")} />
                    )}
                    <div className="relative cursor-pointer" onClick={() => navigate("/cart")}>
                        <ShoppingCart />
                        {cartCount > 0 && (
                            <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
                                {cartCount}
                            </span>
                        )}
                    </div>
                    {isLoggedIn ? (
                        <button
                            onClick={handleLogout}
                            className="flex items-center gap-2 border border-gray-300 px-3 py-1 rounded-lg shadow-sm hover:bg-gray-100 transition"
                        >
                            <LogOut size={18} />
                            Logout
                        </button>
                    ) : (
                        <button
                            onClick={() => navigate("/login")}
                            className="flex items-center gap-2 border border-gray-300 px-3 py-1 rounded-lg shadow-sm hover:bg-gray-100 transition"
                        >
                            <LogIn size={18} />
                            Login
                        </button>
                    )}
                </div>
            </div>
        </div>
    );
}