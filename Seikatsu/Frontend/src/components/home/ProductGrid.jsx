
import ProductCard from "./ProductCard";

export default function ProductGrid({ products }) {
    return (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 pb-16 items-stretch">
            {products.map((p) => (
                <ProductCard key={p.id} product={p} />
            ))}
        </div>
    );
}