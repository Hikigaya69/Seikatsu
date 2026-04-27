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
    const [loadingMore, setLoadingMore] = useState(false);
    const [dropdownOpen, setDropdownOpen] = useState(false);
    const [nextCursorDate, setNextCursorDate] = useState(null);
    const [hasNextPage, setHasNextPage] = useState(false);
    const [totalVisible, setTotalVisible] = useState(0);

    // ✅ Robust universal extractor
    const extractItems = (data) => {
        if (!data) return [];
        if (Array.isArray(data)) return data;
        if (Array.isArray(data.items)) return data.items;
        if (Array.isArray(data.categories)) return data.categories;
        if (Array.isArray(data.results)) return data.results;
        return [];
    };

    useEffect(() => {
        api.get("/Category/getallcategories")
            .then((res) => {
                console.log(" RAW CATEGORY RESPONSE:", res.data);

                const items = extractItems(res.data.data);

                console.log(" EXTRACTED CATEGORIES:", items);
                console.log(" CATEGORY COUNT:", items.length);

                setCategories(items);
            })
            .catch((err) => console.error("❌ CATEGORY ERROR:", err));

        fetchProducts(null, null);
    }, []);

    const fetchProducts = (categoryId, cursorDate) => {
        setLoading(true);

        const endpoint = categoryId
            ? `/Product/category/${categoryId}`
            : `/Product/productforindex`;

        const params = { pageSize: 12 };
        if (cursorDate) {
            params.cursorDate = cursorDate;
        }

        api.get(endpoint, { params })
            .then((res) => {
                const data = res.data.data;
                const items = extractItems(data);

                setProducts(items);
                setNextCursorDate(data?.nextCursorDate || null);
                setHasNextPage(data?.hasNextPage || false);
                setTotalVisible(items.length);
            })
            .catch((err) => console.error(err))
            .finally(() => setLoading(false));
    };

    const loadMore = () => {
        if (!hasNextPage || loadingMore) return;

        setLoadingMore(true);

        const endpoint = selectedCategory
            ? `/Product/category/${selectedCategory}`
            : `/Product/productforindex`;

        const params = {
            pageSize: 12,
            cursorDate: nextCursorDate,
        };

        api.get(endpoint, { params })
            .then((res) => {
                const data = res.data.data;
                const items = extractItems(data);

                setProducts((prev) => [...prev, ...items]);
                setNextCursorDate(data?.nextCursorDate || null);
                setHasNextPage(data?.hasNextPage || false);
                setTotalVisible((prev) => prev + items.length);
            })
            .catch((err) => console.error(err))
            .finally(() => setLoadingMore(false));
    };

    const handleCategorySelect = (category) => {
        if (category === "all") {
            setSelectedCategory(null);
            setSelectedCategoryName("All Categories");
            fetchProducts(null, null);
        } else {
            setSelectedCategory(category.id);
            setSelectedCategoryName(category.categoryName || "Unnamed");
            fetchProducts(category.id, null);
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

                {/* Dropdown */}
                <div className="flex items-center justify-between mb-6">
                    <div className="relative inline-block">

                        <button
                            onClick={() => setDropdownOpen((prev) => !prev)}
                            className="flex items-center gap-2 bg-white border border-gray-300 px-5 py-2.5 rounded-lg shadow-sm hover:shadow-md transition text-sm font-medium text-gray-700"
                        >
                            🗂 {selectedCategoryName}
                            <svg
                                className={`w-4 h-4 transition-transform ${dropdownOpen ? "rotate-180" : ""}`}
                                fill="none"
                                stroke="currentColor"
                                viewBox="0 0 24 24"
                            >
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                            </svg>
                        </button>

                        {dropdownOpen && (
                            <div className="absolute z-50 mt-2 w-80 bg-white border border-gray-200 rounded-xl shadow-lg max-h-80 overflow-y-auto">

                                {/* All Categories */}
                                <button
                                    onClick={() => handleCategorySelect("all")}
                                    className={`w-full text-left px-4 py-2.5 text-sm hover:bg-gray-100 transition flex items-center gap-2
                                        ${selectedCategory === null ? "font-semibold text-[#3c6e71]" : "text-gray-700"}`}
                                >
                                    🛒 <span>All Categories</span>
                                </button>

                                <div className="border-t border-gray-100" />

                                {/* Categories */}
                                {categories.map((cat, index) => (
                                    <button
                                        key={`${cat.id}-${index}`}
                                        onClick={() => handleCategorySelect(cat)}
                                        className={`w-full text-left px-4 py-2.5 text-sm transition hover:bg-gray-100
                                            ${selectedCategory === cat.id
                                                ? "font-semibold text-[#3c6e71] bg-gray-50"
                                                : "text-gray-700"
                                            }`}
                                    >
                                        {cat.categoryName || "Unnamed Category"}
                                    </button>
                                ))}

                                {/* Debug info in UI */}
                                {categories.length === 0 && (
                                    <div className="p-4 text-sm text-gray-400">
                                        No categories found
                                    </div>
                                )}
                            </div>
                        )}
                    </div>

                    {/* Counter */}
                    {!loading && products.length > 0 && (
                        <span className="text-sm text-gray-500">
                            Showing <span className="font-semibold text-gray-700">{totalVisible}</span> products
                            {hasNextPage && " · more available"}
                        </span>
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
                    <>
                        <ProductGrid products={products} />

                        <div className="flex flex-col items-center gap-2 mt-10 mb-6">
                            {hasNextPage ? (
                                <button
                                    onClick={loadMore}
                                    disabled={loadingMore}
                                    className="px-8 py-2.5 bg-[#3c6e71] text-white text-sm font-medium rounded-lg hover:bg-[#2f5a5c] transition disabled:opacity-50"
                                >
                                    {loadingMore ? "Loading..." : "Load more"}
                                </button>
                            ) : (
                                <p className="text-sm text-gray-400">You have reached the end</p>
                            )}
                        </div>
                    </>
                )}
            </div>

            <Footer />
        </div>
    );
}