import { ShoppingCart, User, LogOut } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import axios from "axios";

export default function HomeNavbar() {
    const navigate = useNavigate();
    const [cartCount, setCartCount] = useState(0);

    useEffect(() => {
        const fetchCartSummary = async () => {
            try {
                const res = await axios.get("/api/Cart/cartsummary", {
                    withCredentials: true
                });
                setCartCount(res.data.data.totalItems);
            } catch (err) {
                console.error(err);
            }
        };
        fetchCartSummary();
    }, []);

    const handleLogout = async () => {
        try {
            await axios.post("/api/Auth/logout", {}, {
                withCredentials: true
            });
            navigate("/login");
        } catch (err) {
            console.error(err);
        }
    };

    return (
        <div className="bg-white border-b p-2 shadow-sm">
            <div className="max-w-7xl mx-auto flex items-center justify-between p-5">
                <h1 className="text-2xl font-bold text-[#284b63]">Seikatsu</h1>

                <input
                    type="text"
                    placeholder="Search product"
                    className="w-125 px-4 py-2 border rounded-lg"
                />

                <div className="flex gap-6 items-center">
                    <User className="cursor-pointer" />
                    <div className="relative cursor-pointer" onClick={() => navigate("/cart")}>
                        <ShoppingCart />
                        {cartCount > 0 && (
                            <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
                                {cartCount}
                            </span>
                        )}
                    </div>
                    <button
                        onClick={handleLogout}
                        className="flex items-center gap-2 border border-gray-300 px-3 py-1 rounded-lg shadow-sm hover:bg-gray-100 transition"
                    >
                        <LogOut size={18} />
                        Logout
                    </button>
                </div>
            </div>
        </div>
    );
}