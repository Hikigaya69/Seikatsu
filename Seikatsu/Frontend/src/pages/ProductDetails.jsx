import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import StoreLayout from "../layouts/StoreLayout";
import { ArrowLeft, Repeat } from "lucide-react";
import api from "../Utils/api";

export default function ProductDetails() {

    const { id } = useParams();
    const [product, setProduct] = useState(null);
    const navigate = useNavigate();

    const [addedMessage, setAddedMessage] = useState(false);
    const [restockMessage, setRestockMessage] = useState(false);

    const [quantity, setQuantity] = useState(1);
    const [frequency, setFrequency] = useState(0);

    const [isLoggedIn, setIsLoggedIn] = useState(false);

    const increaseQty = () => setQuantity(prev => prev + 1);

    const decreaseQty = () => {
        if (quantity > 1) setQuantity(prev => prev - 1);
    };

    const addToCart = async () => {

        if (!isLoggedIn) {
            navigate("/login");
            return;
        }

        try {

            await api.post(
                "/Cart/additem",
                { productId: product.id, quantity },
                { withCredentials: true }
            );

            setAddedMessage(true);

        } catch (err) {

            console.error(err.response?.data);

        }
    };

    const addToRestockCart = async () => {

        if (!isLoggedIn) {
            navigate("/login");
            return;
        }

        try {

            await api.post(
                "/RestockCart/additem-restockcart",
                {
                    productId: product.id,
                    quantity: 1,
                    frequency
                },
                { withCredentials: true }
            );

            setRestockMessage(true);

        } catch (err) {

            console.error(err.response?.data);

        }
    };

    useEffect(() => {

        const checkAuth = async () => {

            try {

                await api.get("/Auth/check", {
                    withCredentials: true
                });

                setIsLoggedIn(true);

            } catch {

                setIsLoggedIn(false);

            }
        };

        const fetchProduct = async () => {

            try {

                const res =
                    await api.get(`/Product/productview/${id}`);

                setProduct(res.data.data);

            } catch (err) {

                console.error(err);

            }
        };

        checkAuth();
        fetchProduct();

    }, [id]);

    if (!product) {

        return (

            <StoreLayout>

                <div className="max-w-7xl mx-auto px-6 pt-6">

                    <button
                        onClick={() => navigate(-1)}
                        className="flex items-center gap-2 border border-gray-300 px-4 py-2 rounded-lg shadow-sm hover:shadow-md hover:bg-gray-50 transition"
                    >
                        <ArrowLeft size={18} />
                        Go Back
                    </button>

                </div>

                <div className="p-10 text-gray-500">
                    Product not found.
                </div>

            </StoreLayout>
        );
    }

    return (

        <StoreLayout>

            <div className="max-w-7xl mx-auto px-6 pt-6">

                <button
                    onClick={() => navigate(-1)}
                    className="flex items-center gap-2 border border-gray-300 px-4 py-2 rounded-lg shadow-sm hover:shadow-md hover:bg-gray-50 transition"
                >
                    <ArrowLeft size={18} />
                    Go Back
                </button>

            </div>

            <div className="max-w-7xl mx-auto px-6 py-10">

                <div className="grid grid-cols-2 gap-10">

                    <div className="bg-white p-6 rounded-xl shadow">

                        <img
                            src={product.productImageUrl || "/ramen.jpg"}
                            className="w-full h-100 object-cover rounded-lg"
                        />

                    </div>

                    <div>

                        <h1 className="text-3xl font-bold mb-4">
                            {product.name}
                        </h1>

                        <p className="text-gray-500 mb-4">
                            {product.description}
                        </p>

                        <div className="text-2xl font-bold text-[#284b63] mb-6">
                            ₹{product.price}
                        </div>

                        <div className="mb-2 text-sm text-gray-600">
                            Category: {product.category}
                        </div>

                        <div className="mb-6 text-sm text-gray-600">
                            Country: {product.countryName}
                        </div>

                        <div className="flex items-center gap-4 mt-6">

                            <div className="flex items-center border rounded-lg overflow-hidden">

                                <button
                                    onClick={decreaseQty}
                                    className="px-4 py-2 bg-gray-200 hover:bg-gray-300"
                                >
                                    −
                                </button>

                                <span className="px-6 py-2 font-medium">
                                    {quantity}
                                </span>

                                <button
                                    onClick={increaseQty}
                                    className="px-4 py-2 bg-gray-200 hover:bg-gray-300"
                                >
                                    +
                                </button>

                            </div>

                            <button
                                onClick={addToCart}
                                disabled={addedMessage}
                                className="flex-1 bg-[#3c6e71] text-white px-6 py-3 rounded-lg hover:bg-[#2f5557] transition disabled:opacity-60"
                            >
                                {isLoggedIn
                                    ? "Add to Cart"
                                    : "Login to Add to Cart"}
                            </button>

                        </div>

                        {addedMessage && (

                            <p className="text-green-600 mt-3">
                                ✅ Item added to cart successfully
                            </p>

                        )}

                        <div className="mt-8 border-t pt-6">

                            <div className="flex items-center gap-4">

                                <select
                                    value={frequency}
                                    onChange={e =>
                                        setFrequency(Number(e.target.value))
                                    }
                                    className="border px-3 py-2 rounded-lg"
                                >
                                    <option value={0}>
                                        Weekly
                                    </option>

                                    <option value={1}>
                                        Biweekly
                                    </option>

                                    <option value={2}>
                                        Monthly
                                    </option>

                                </select>

                                <button
                                    onClick={addToRestockCart}
                                    className="flex items-center gap-2 bg-purple-100 text-purple-700 px-6 py-3 rounded-lg hover:bg-purple-200 transition"
                                >
                                    <Repeat size={18} />
                                    Add to Restock Cart
                                </button>

                            </div>

                            {restockMessage && (

                                <p className="text-purple-600 mt-3">
                                    🔁 Added to restock cart successfully
                                </p>

                            )}

                        </div>

                    </div>

                </div>

            </div>

        </StoreLayout>
    );
}