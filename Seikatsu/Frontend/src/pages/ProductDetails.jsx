import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import StoreLayout from "../layouts/StoreLayout";
import { useNavigate } from "react-router-dom";
import { ArrowLeft } from "lucide-react";
import axios from "axios";
import api from "../lib/api";


export default function ProductDetails() {

    const { id } = useParams();
    const [product, setProduct] = useState(null);
    const navigate = useNavigate();
    const [addedMessage, setAddedMessage] = useState(false);
    const [quantity, setQuantity] = useState(1);
    const increaseQty = () => {
        setQuantity((prev) => prev + 1);
    };

    const decreaseQty = () => {
        if (quantity > 1) {
            setQuantity((prev) => prev - 1);
        }
    };
    ; const addToCart = async () => {

        try {

            await api.post(
                "/api/Cart/additem",
                { productId: product.id, quantity },
                { withCredentials: true }
            );

            setAddedMessage(true);



        } catch (err) {

            console.error("Error response:", err.response?.data);

        }

    };
    useEffect(() => {

        const fetchProduct = async () => {

            try {

                const res = await axios.get(
                    `/api/Product/productview/${id}`

                );
                console.log("API Response:", res.data);

                setProduct(res.data.data);

            } catch (err) {
                console.error(err);
            }

        };

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

                    {/* IMAGE */}
                    <div className="bg-white p-6 rounded-xl shadow">

                        <img
                            src={product.productImageUrl || "/ramen.jpg"}
                            className="w-full h-100 object-cover rounded-lg"
                        />

                    </div>

                    {/* INFO */}
                    <div>

                        <h1 className="text-3xl font-bold mb-4">
                            {product.name}
                        </h1>

                        <p className="text-gray-500 mb-4">
                            {product.description}
                        </p>

                        <div className="text-2xl font-bold text-[#284b63] mb-6">
                            ¥{product.price}
                        </div>

                        <div className="mb-2 text-sm text-gray-600">
                            Category: {product.category}
                        </div>

                        <div className="mb-6 text-sm text-gray-600">
                            Country: {product.countryName}
                        </div>

                        <div className="flex items-center gap-4 mt-6">

                            {/* Quantity select */}
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

                            {/* Cart button */}
                            <button
                                onClick={addToCart}
                                disabled={addedMessage}
                                className="flex-1 bg-[#3c6e71] text-white px-6 py-3 rounded-lg hover:bg-[#2f5557] transition disabled:opacity-60"
                            >
                                Add to Cart
                            </button>
                            {addedMessage && (
                                <p className="text-green-600 mt-3">
                                    ✅ Item added to cart successfully
                                </p>
                            )}


                        </div>

                    </div>

                </div>

            </div></StoreLayout>

    );
}