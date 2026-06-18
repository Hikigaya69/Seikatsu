import {
    ShoppingCart,
    User,
    LogOut,
    LogIn,
    ClipboardList,
    Repeat,
    Search,
    X
} from "lucide-react";

import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import api from "../../Utils/api";

export default function HomeNavbar() {

    const navigate = useNavigate();

    const [cartCount, setCartCount] = useState(0);
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [checklistCount, setChecklistCount] = useState(0);

    // SEARCH STATES
    const [query, setQuery] = useState("");
    const [suggestions, setSuggestions] = useState([]);
    const [showSuggestions, setShowSuggestions] = useState(false);

    // AUTH + COUNTS
    useEffect(() => {

        const checkAuthAndCart = async () => {

            try {

                await api.get("/Auth/check");

                setIsLoggedIn(true);

                const cartRes =
                    await api.get("/Cart/cartsummary");

                setCartCount(
                    cartRes.data.data.totalItems
                );

                const checklistRes =
                    await api.get("/Checklist/getchecklist");

                setChecklistCount(
                    checklistRes.data.data.items.length
                );

            } catch {

                setIsLoggedIn(false);

            }
        };

        checkAuthAndCart();

    }, []);

    // SEARCH SUGGESTIONS
    useEffect(() => {

        if (!query.trim()) {
            setSuggestions([]);
            return;
        }

        const delay = setTimeout(() => {
            fetchSuggestions();
        }, 300);

        return () => clearTimeout(delay);

    }, [query]);

    const fetchSuggestions = async () => {

        try {

            const res = await api.get(
                `/Product/suggestion/${query}`
            );

            setSuggestions(res.data.data || []);
            setShowSuggestions(true);

        } catch (err) {

            console.error(err);

        }
    };

    // SEARCH NAVIGATION
    const handleSearch = (searchText) => {

        if (!searchText.trim()) return;

        navigate(`/search/${searchText}`);
        setShowSuggestions(false);
    };

    // CART
    const handleCartClick = () => {

        if (!isLoggedIn) {

            navigate("/login");

        } else {

            navigate("/cart");

        }
    };

    // LOGOUT
    const handleLogout = async () => {

        try {

            await api.post(
                "/Auth/logout",
                {},
                { withCredentials: true }
            );

            setIsLoggedIn(false);
            setCartCount(0);

            navigate("/home");

        } catch (err) {

            console.error(err);

        }
    };

    return (

        <nav className="relative h-[78px] overflow-visible border-b shadow-sm">

            {/* GIF BACKGROUND */}
            <img
                src="/GIF/wildlife mt GIF.gif"
                alt="bg"
                className="absolute inset-0 w-full h-full object-cover"
            />

            {/* OVERLAY */}
            <div className="absolute inset-0 bg-black/30 backdrop-blur-sm" />

            {/* CONTENT */}
            <div className="
                relative
                z-10
                h-full
                max-w-7xl
                mx-auto
                px-6
                flex
                items-center
                justify-between
            ">

                {/* LOGO */}
                <h1
                    onClick={() => navigate("/home")}
                    className="
                        text-3xl
                        font-bold
                        tracking-wide
                        text-white
                        cursor-pointer
                        hover:opacity-90
                        transition
                    "
                >
                    Seikatsu
                </h1>

                {/* SEARCH */}
                <div className="relative w-[430px]">

                    {/* INPUT */}
                    <div className="relative">

                        <Search
                            size={18}
                            className="
                                absolute
                                left-4
                                top-1/2
                                -translate-y-1/2
                                text-gray-400
                            "
                        />

                        <input
                            type="text"
                            placeholder="Search Japanese snacks, ramen, drinks..."
                            value={query}
                            onChange={(e) => setQuery(e.target.value)}
                            onKeyDown={(e) => {
                                if (e.key === "Enter") {
                                    handleSearch(query);
                                }
                            }}
                            className="
                                w-full
                                pl-11
                                pr-10
                                py-3
                                rounded-2xl
                                bg-white/90
                                backdrop-blur-md
                                border
                                border-white/20
                                shadow-xl
                                text-sm
                                focus:outline-none
                                focus:ring-2
                                focus:ring-[#3c6e71]
                            "
                        />

                        {query && (

                            <X
                                size={16}
                                onClick={() => {
                                    setQuery("");
                                    setSuggestions([]);
                                }}
                                className="
                                    absolute
                                    right-4
                                    top-1/2
                                    -translate-y-1/2
                                    text-gray-400
                                    cursor-pointer
                                    hover:text-black
                                "
                            />

                        )}

                    </div>

                    {/* SUGGESTIONS */}
                    {showSuggestions && suggestions.length > 0 && (

                        <div className="
                            absolute
                            top-full
                            mt-3
                            w-full
                            bg-white
                            rounded-2xl
                            shadow-2xl
                            border
                            border-gray-100
                            overflow-hidden
                            z-50
                        ">

                            {suggestions.map((item) => (

                                <button
                                    key={item.id}
                                    onClick={() =>
                                        handleSearch(item.name)
                                    }
                                    className="
                                        w-full
                                        text-left
                                        px-5
                                        py-4
                                        hover:bg-gray-50
                                        transition
                                        border-b
                                        border-gray-100
                                        flex
                                        items-center
                                        gap-3
                                    "
                                >

                                    <Search
                                        size={16}
                                        className="text-gray-400"
                                    />

                                    <span className="text-sm text-gray-700">
                                        {item.name}
                                    </span>

                                </button>

                            ))}

                        </div>

                    )}

                </div>

                {/* RIGHT SIDE */}
                <div className="flex gap-6 items-center">

                    {/* PROFILE */}
                    {isLoggedIn && (

                        <User
                            className="
                                cursor-pointer
                                text-white
                                hover:opacity-70
                                transition
                            "
                            onClick={() => navigate("/profile")}
                        />

                    )}

                    {/* CHECKLIST */}
                    {isLoggedIn && (

                        <div
                            className="relative cursor-pointer"
                            onClick={() => navigate("/checklist")}
                        >

                            <ClipboardList
                                className="
                                    text-white
                                    hover:opacity-70
                                    transition
                                "
                            />

                            {checklistCount > 0 && (

                                <span className="
                                    absolute
                                    -top-2
                                    -right-2
                                    bg-blue-500
                                    text-white
                                    text-xs
                                    rounded-full
                                    w-5
                                    h-5
                                    flex
                                    items-center
                                    justify-center
                                ">
                                    {checklistCount}
                                </span>

                            )}

                        </div>

                    )}

                    {/* RESTOCK */}
                    {isLoggedIn && (

                        <div
                            title="Restock Cart"
                            className="cursor-pointer"
                            onClick={() => navigate("/restock-cart")}
                        >

                            <Repeat
                                className="
                                    text-white
                                    hover:opacity-70
                                    transition
                                "
                            />

                        </div>

                    )}

                    {/* CART */}
                    <div
                        className="relative cursor-pointer"
                        onClick={handleCartClick}
                    >

                        <ShoppingCart
                            className="
                                text-white
                                hover:opacity-70
                                transition
                            "
                        />

                        {cartCount > 0 && (

                            <span className="
                                absolute
                                -top-2
                                -right-2
                                bg-red-500
                                text-white
                                text-xs
                                rounded-full
                                w-5
                                h-5
                                flex
                                items-center
                                justify-center
                            ">
                                {cartCount}
                            </span>

                        )}

                    </div>

                    {/* AUTH BUTTON */}
                    {isLoggedIn ? (

                        <button
                            onClick={handleLogout}
                            className="
                                flex
                                items-center
                                gap-2
                                border
                                border-white/40
                                px-4
                                py-2
                                rounded-xl
                                bg-white/10
                                backdrop-blur-md
                                text-white
                                hover:bg-white/20
                                transition
                            "
                        >

                            <LogOut size={18} />
                            Logout

                        </button>

                    ) : (

                        <button
                            onClick={() => navigate("/login")}
                            className="
                                flex
                                items-center
                                gap-2
                                border
                                border-white/40
                                px-4
                                py-2
                                rounded-xl
                                bg-white/10
                                backdrop-blur-md
                                text-white
                                hover:bg-white/20
                                transition
                            "
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