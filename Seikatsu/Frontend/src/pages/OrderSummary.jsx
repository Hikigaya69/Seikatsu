import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import api from "../Utils/api";
import StoreLayout from "../layouts/StoreLayout";
import { ArrowLeft } from "lucide-react";
import { useNavigate } from "react-router-dom";

export default function OrderSummary() {

    const { orderId } = useParams();
    const [summary, setSummary] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {

        const fetchSummary = async () => {

            const res = await api.get(
                `Order/ordersummarybyOrderId?orderId=${orderId}`,
                { withCredentials: true }
            );

            setSummary(res.data.data);

        };

        fetchSummary();

    }, [orderId]);

    if (!summary)
        return <StoreLayout>Loading...</StoreLayout>;

    return (

        <StoreLayout>

            <div className="max-w-6xl mx-auto px-6 py-10">
                <div className="max-w-6xl mx-auto px-6 pt-6">

                    <button
                        onClick={() => navigate(-1)}
                        className="flex items-center gap-2 border border-gray-300 px-4 py-2 rounded-lg shadow-sm hover:shadow-md hover:bg-gray-50 transition"
                    >
                        <ArrowLeft size={18} />
                        Go Back
                    </button>

                </div>
                <h1 className="text-2xl font-bold mb-6">
                    Order Details
                </h1>


                {/* ADDRESS */}
                <div className="bg-white shadow rounded-xl p-6 mb-6">

                    <h2 className="font-semibold mb-2">
                        Delivery Address
                    </h2>

                    <p>{summary.deliveryAddress.fullName}</p>
                    <p>{summary.deliveryAddress.addressLine1}</p>
                    <p>{summary.deliveryAddress.city}</p>
                    <p>{summary.deliveryAddress.postalCode}</p>

                </div>


                {/* ITEMS */}
                <div className="space-y-4">

                    {summary.items.map(item => (

                        <div
                            key={item.productId}
                            className="bg-white shadow rounded-xl p-4 flex justify-between"
                        >

                            <div className="flex gap-4">

                                <img
                                    src={item.productImageUrl}
                                    className="w-16 h-16 rounded"
                                />

                                <div>

                                    <p className="font-semibold">
                                        {item.productName}
                                    </p>

                                    <p className="text-gray-500">
                                        Qty: {item.quantity}
                                    </p>

                                </div>

                            </div>

                            <p className="font-semibold">
                                ¥{item.lineTotal}
                            </p>

                        </div>

                    ))}

                </div>


                {/* BILL SUMMARY */}
                <div className="bg-white shadow rounded-xl p-6 mt-6">

                    <p>Subtotal: ¥{summary.subTotal}</p>
                    <p>Delivery: ¥{summary.deliveryCharge}</p>
                    <p>Tax: ¥{summary.tax}</p>

                    <hr className="my-2" />

                    <p className="font-bold text-lg">
                        Total: ¥{summary.totalAmount}
                    </p>

                    <p className="text-sm text-gray-500 mt-2">
                        Status: {summary.orderStatus}
                    </p>

                </div>

            </div>

        </StoreLayout>

    );

}