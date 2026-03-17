import ProductCard from "./ProductCard";

export default function ProductGrid({ products }) {

  return (

    <div className="grid grid-cols-4 gap-8 pb-16">

      {products.map((p) => (
        <ProductCard key={p.id} product={p} />
      ))}

    </div>

  );
}