import { ShoppingCart, User, LogOut, LogIn } from "lucide-react";
import { ClipboardList } from "lucide-react";
import { Repeat } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import api from "../../Utils/api";

export default function HomeNavbar() {
    const navigate = useNavigate();
    const [cartCount, setCartCount] = useState(0);
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [checklistCount, setChecklistCount] = useState(0);

    useEffect(() => {
        const checkAuthAndCart = async () => {
    try {

        await api.get("/Auth/check");

        setIsLoggedIn(true);

        const cartRes = await api.get("/Cart/cartsummary");
        setCartCount(cartRes.data.data.totalItems);

        const checklistRes = await api.get("/Checklist/getchecklist");
        setChecklistCount(checklistRes.data.data.items.length);

    } catch {
        setIsLoggedIn(false);
    }
};
        checkAuthAndCart();
    }, []);

    const handleCartClick = () => {
        if (!isLoggedIn) {
            navigate("/login");
        } else {
            navigate("/cart");
        }
    };

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
        <nav className="relative h-[70px] overflow-hidden border-b shadow-sm">

            {/* GIF Background */}
            <img
                src="/GIF/wildlife mt GIF.gif"
                alt="bg"
                className="absolute inset-0 w-full h-full object-cover"
            />

            {/* Semi transparent overlay so text is readable over GIF */}
            <div className="absolute inset-0 backdrop-blur-sm" />

            {/* Navbar content */}
            <div className="relative z-10 h-full max-w-7xl mx-auto px-6 flex items-center justify-between">

                <h1
                    onClick={() => navigate("/home")}
                    className="text-2xl font-bold text-white cursor-pointer hover:opacity-100 transition"
                >
                    Seikatsu
                </h1>

                <input
                    type="text"
                    placeholder="Search product"
                    className="w-96 px-4 py-2 border rounded-lg bg-white/80 focus:outline-none focus:ring-2 focus:ring-[#3c6e71]"
                />

                <div className="flex gap-6 items-center">
                    {isLoggedIn && (
                        <User
                            className="cursor-pointer text-white hover:opacity-50 transition"
                            onClick={() => navigate("/profile")}
                        />
                    )}
                    {isLoggedIn && (
    <div
        className="relative cursor-pointer"
        onClick={() => navigate("/checklist")}
    >
        <ClipboardList className="text-white hover:opacity-70 transition" />

        {checklistCount > 0 && (
            <span className="absolute -top-2 -right-2 bg-blue-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
                {checklistCount}
            </span>
        )}
    </div>
)}

{isLoggedIn && (
    <div
        title="Restock Cart"
        className="cursor-pointer"
        onClick={() => navigate("/restock-cart")}
    >
        <Repeat className="text-white hover:opacity-70 transition" />
    </div>
)}

                    <div className="relative cursor-pointer" onClick={handleCartClick}>
                        <ShoppingCart className="text-white hover:opacity-70 transition" />
                        {cartCount > 0 && (
                            <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
                                {cartCount}
                            </span>
                        )}
                    </div>

                    {isLoggedIn ? (
                        <button
                            onClick={handleLogout}
                            className="flex items-center gap-2 border border-gray-300 px-3 py-1 rounded-lg shadow-sm hover:bg-white/80 transition text-white"
                        >
                            <LogOut size={18} />
                            Logout
                        </button>
                    ) : (
                        <button
                            onClick={() => navigate("/login")}
                            className="flex items-center gap-2 border-3 border-gray-300 px-3 py-1 rounded-lg shadow-sm hover:bg-white/30 transition text-white"
                        >
                            <LogIn size={18} />
                            Login
                        </button>
                    )}
                </div>
            </div>
        </nav>
    );
}