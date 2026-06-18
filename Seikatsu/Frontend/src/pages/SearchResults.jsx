import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { ArrowLeft } from "lucide-react";
import { useNavigate } from "react-router-dom";

import api from "../Utils/api";

import HomeNavbar from "../components/home/HomeNavbar";
import ProductGrid from "../components/home/ProductGrid";
import Footer from "../components/Footer";

export default function SearchResults() {

    const { query } = useParams();

    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {

        fetchProducts();

    }, [query]);

    const fetchProducts = async () => {

        try {

            setLoading(true);

            const res = await api.get(
                `/Product/search/${query}`
            );

            setProducts(res.data.data || []);

        } catch (err) {

            console.error(err);

        } finally {

            setLoading(false);

        }
    };

    return (

        <div className="min-h-screen bg-[#f8fafc]">

            <HomeNavbar />
            <div className="max-w-7xl mx-auto px-6 pt-6">

    <button
        onClick={() => navigate("/home")}
        className="
            flex
            items-center
            gap-2
            border
            border-gray-300
            px-5
            py-2.5
            rounded-xl
            bg-white
            shadow-sm
            hover:shadow-md
            hover:bg-gray-50
            transition
        "
    >

        <ArrowLeft size={18} />

        Back to Home

    </button>

</div>

            <div className="max-w-7xl mx-auto px-6 py-10">

                <div className="mb-10">

                    <h1 className="text-4xl font-bold text-gray-800">
                        Search Results
                    </h1>

                    <p className="text-gray-500 mt-2">
                        Showing products for:
                        <span className="font-semibold text-[#3c6e71]">
                            {" "} {query}
                        </span>
                    </p>

                </div>

                {loading ? (

                    <div className="py-20 text-center text-gray-400">
                        Loading products...
                    </div>

                ) : products.length === 0 ? (

                    <div className="py-24 text-center">

                        <h2 className="text-3xl font-semibold text-gray-700">
                            No Products Found
                        </h2>

                        <p className="text-gray-400 mt-3">
                            Try another search keyword
                        </p>

                    </div>

                ) : (

                    <ProductGrid products={products} />

                )}

            </div>

            <Footer />

        </div>

    );
}