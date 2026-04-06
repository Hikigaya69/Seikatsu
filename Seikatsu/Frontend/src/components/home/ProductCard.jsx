// ProductCard.jsx
import { useNavigate } from "react-router-dom";

export default function ProductCard({ product }) {
    const navigate = useNavigate();

    return (
        <div
            onClick={() => navigate(`/product/${product.id}`)}
            className="bg-white shadow rounded-xl overflow-hidden hover:shadow-lg transition cursor-pointer flex flex-col h-full"
        >
            {/* Image */}
            <div className="w-full h-48 bg-gray-100 overflow-hidden">
                <img
                    src={product.productImageUrl || "https://placehold.co/400x300?text=No+Image"}
                    alt={product.name}
                    className="w-full h-full object-cover hover:scale-105 transition-transform duration-300"
                />
            </div>

            {/* Content */}
            <div className="p-4 flex flex-col flex-1">

                <h3 className="font-semibold text-gray-800 text-base leading-snug line-clamp-2">
                    {product.name}
                </h3>

                <p className="text-gray-400 text-xs mt-1 mb-3 line-clamp-2">
                    {product.description}
                </p>

                {/* Push price + button to bottom */}
                <div className="flex justify-between items-center mt-auto pt-3 border-t border-gray-100">
                    <span className="text-[#284b63] font-bold text-lg">
                        ₹{product.price}
                    </span>
                    <button
                        className="bg-[#3c6e71] text-white text-sm px-4 py-1.5 rounded-lg hover:bg-[#2f5557] transition"
                        onClick={(e) => {
                            e.stopPropagation();
                            navigate(`/product/${product.id}`);
                        }}
                    >
                        View Details
                    </button>
                </div>

            </div>
        </div>
    );
}