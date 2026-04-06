import { useEffect, useState } from "react";
import HomeNavbar from "../components/home/HomeNavbar";
import HeroBanner from "../components/home/HeroBanner";
import ProductGrid from "../components/home/ProductGrid";
import Footer from "../components/Footer";
import api from "../Utils/api";

export default function Home() {
    const [products, setProducts] = useState([]);
    const [categories, setCategories] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState(null);
    const [selectedCategoryName, setSelectedCategoryName] = useState("All Categories");
    const [loading, setLoading] = useState(false);
    const [dropdownOpen, setDropdownOpen] = useState(false);

    // Fetch categories + initial products on mount
    useEffect(() => {
        api.get("/Category/getallcategories").then((res) => {
            setCategories(res.data.data);
        });

        fetchProducts(null);
    }, []);

    const fetchProducts = (categoryId) => {
        setLoading(true);
        const endpoint = categoryId
            ? `/Product/category/${categoryId}`
            : "/Product/productforindex";

        api.get(endpoint)
            .then((res) => setProducts(res.data.data))
            .catch((err) => console.error(err))
            .finally(() => setLoading(false));
    };

    const handleCategorySelect = (category) => {
        if (category === "all") {
            setSelectedCategory(null);
            setSelectedCategoryName("All Categories");
            fetchProducts(null);
        } else {
            setSelectedCategory(category.id);
            setSelectedCategoryName(category.categoryName);
            fetchProducts(category.id);
        }
        setDropdownOpen(false);
    };

    return (
        <div className="min-h-screen">
            <HomeNavbar />
            <div className="p-7">
                <HeroBanner />
            </div>

            <div className="max-w-7xl mx-auto px-6">

                {/* Category Dropdown */}
                <div className="relative inline-block mb-6">
                    <button
                        onClick={() => setDropdownOpen((prev) => !prev)}
                        className="flex items-center gap-2 bg-white border border-gray-300 px-5 py-2.5 rounded-lg shadow-sm hover:shadow-md transition text-sm font-medium text-gray-700"
                    >
                        🗂 {selectedCategoryName}
                        <svg
                            className={`w-4 h-4 transition-transform ${dropdownOpen ? "rotate-180" : ""}`}
                            fill="none" stroke="currentColor" viewBox="0 0 24 24"
                        >
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                        </svg>
                    </button>

                    {dropdownOpen && (
                        <div className="absolute z-50 mt-2 w-64 bg-white border border-gray-200 rounded-xl shadow-lg max-h-80 overflow-y-auto">

                            <button
                                onClick={() => handleCategorySelect("all")}
                                className={`w-full text-left px-4 py-2.5 text-sm hover:bg-gray-50 transition
                                    ${selectedCategory === null ? "font-semibold text-[#3c6e71]" : "text-gray-700"}`}
                            >
                                🛒 All Categories
                            </button>

                            <div className="border-t border-gray-100" />

                            {categories.map((cat) => (
                                <button
                                    key={cat.id}
                                    onClick={() => handleCategorySelect(cat)}
                                    className={`w-full text-left px-4 py-2.5 text-sm hover:bg-gray-50 transition
                                        ${selectedCategory === cat.id ? "font-semibold text-[#3c6e71] bg-gray-50" : "text-gray-700"}`}
                                >
                                    {cat.categoryName}
                                </button>
                            ))}
                        </div>
                    )}
                </div>

                {/* Products */}
                {loading ? (
                    <div className="flex justify-center items-center py-20 text-gray-400 text-sm">
                        Loading products...
                    </div>
                ) : products.length === 0 ? (
                    <div className="flex justify-center items-center py-20 text-gray-400 text-sm">
                        No products found in this category.
                    </div>
                ) : (
                    <ProductGrid products={products} />
                )}

            </div>
            <Footer />
        </div>
    );
}